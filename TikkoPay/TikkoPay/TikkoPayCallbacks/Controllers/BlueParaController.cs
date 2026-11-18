using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Entities;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Controllers
{
    /// <summary>
    /// BluePara yatırım/çekim durum bildirimi.
    /// Güvenlik: 1) X-Signature imzası doğrulanır. 2) Bildirimdeki durum ve tutara güvenilmez; işlem BluePara'nın
    /// sorgulama servisinden tekrar okunur ve kayıt eşleştirmesi oradan gelen bilgiyle yapılır. Böylece başka bir
    /// işlemin bildirimi kopyalanıp tekrar gönderilse bile yanlış kayıt onaylanamaz.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BlueParaController : ControllerBase
    {
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IBlueParaApiClient _blueParaApiClient;
        private readonly IBlueParaDepositCallbackService _blueParaDepositCallbackService;
        private readonly IBlueParaWithdrawCallbackService _blueParaWithdrawCallbackService;

        public BlueParaController(ISettingsV2Repository settingsV2Repository,
                                  IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                  IBlueParaApiClient blueParaApiClient,
                                  IBlueParaDepositCallbackService blueParaDepositCallbackService,
                                  IBlueParaWithdrawCallbackService blueParaWithdrawCallbackService)
        {
            _settingsV2Repository = settingsV2Repository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _blueParaApiClient = blueParaApiClient;
            _blueParaDepositCallbackService = blueParaDepositCallbackService;
            _blueParaWithdrawCallbackService = blueParaWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(CancellationToken cancellationToken)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();
            if (settings.BlueParaTedarikciId == Guid.Empty)
            {
                return Fail("BluePara ayarları tanımlı değil.");
            }

            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync(cancellationToken);

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(rawBody);
            }
            catch (JsonException)
            {
                return Fail("Invalid JSON");
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    return Fail("Invalid JSON");
                }

                if (!VerifySignature(root, Request.Headers["X-Signature"].ToString(), settings))
                {
                    return Fail("Invalid webhook signature");
                }

                var transactionId = GetString(root, "transaction_id");
                var type = GetString(root, "type");
                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    return Fail("Invalid transaction");
                }

                // Yatırım linki (generate-link) ile açılan işlemler "gateway" tipiyle bildirilir
                if (string.Equals(type, "deposit", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(type, "gateway", StringComparison.OrdinalIgnoreCase))
                {
                    return await HandleDeposit(transactionId, settings);
                }

                if (string.Equals(type, "withdrawal", StringComparison.OrdinalIgnoreCase))
                {
                    return await HandleWithdraw(transactionId, settings);
                }

                // Kart ödemesi vb. kullanmadığımız tipler
                return Ok(new { status = "success", message = "Ignored" });
            }
        }

        private async Task<IActionResult> HandleDeposit(string transactionId, Settings settings)
        {
            var info = await _blueParaApiClient.GetDepositAsync(transactionId, settings);
            if (info is null)
            {
                // BluePara'ya ulaşılamadı; 2xx dönülmez ki bildirimi tekrar göndersin.
                return StatusCode(StatusCodes.Status502BadGateway, new { status = "error", message = "BluePara işlem sorgulanamadı" });
            }

            var status = MapStatus(info.Status);
            if (status is null)
            {
                return Ok(new { status = "success", message = "Pending, ignored" });
            }

            // Öncelik callback_url'deki ref; yoksa BluePara'dan okunan merchant_transaction_id ("{transactionId}-{zaman}")
            var webSiteTransactionId = GetRefFromCallbackUrl(info.CallbackUrl);
            if (string.IsNullOrWhiteSpace(webSiteTransactionId))
            {
                webSiteTransactionId = GetRefFromMerchantTransactionId(info.MerchantTransactionId);
            }

            if (string.IsNullOrWhiteSpace(webSiteTransactionId))
            {
                return Fail("Yatirim kaydı bulunamadı!");
            }

            var item = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.BlueParaTedarikciId, webSiteTransactionId);
            if (item is null || item.IslemTipi != (int)IslemTip.Yatirim)
            {
                return Fail("Yatirim kaydı bulunamadı!");
            }

            if (item.Durum == (int)Durum.Onaylandi || (item.Durum == (int)Durum.Red && status == (int)Durum.Red))
            {
                return Ok(new { status = "success", message = "Already processed" });
            }

            var result = await _blueParaDepositCallbackService.ProcessDeposit(status.Value, info.Amount, item, settings);
            return BuildResponse(result);
        }

        private async Task<IActionResult> HandleWithdraw(string transactionId, Settings settings)
        {
            var info = await _blueParaApiClient.GetWithdrawalAsync(transactionId, settings);
            if (info is null)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { status = "error", message = "BluePara işlem sorgulanamadı" });
            }

            var status = MapStatus(info.Status);
            if (status is null)
            {
                return Ok(new { status = "success", message = "Pending, ignored" });
            }

            // merchant_transaction_id = "{talepId:N}-{zaman}"
            var merchantTransactionId = info.MerchantTransactionId ?? string.Empty;
            if (merchantTransactionId.Length < 32 || !Guid.TryParseExact(merchantTransactionId[..32], "N", out var cekimTalebiId))
            {
                return Fail("Çekim kaydı bulunamadı!");
            }

            var item = await _yatirimCekimTalepleriRepository.GetById(cekimTalebiId);
            if (item is null || item.SilindiMi || item.IslemTipi != (int)IslemTip.Cekim || item.TedarikciId != settings.BlueParaTedarikciId)
            {
                return Fail("Çekim kaydı bulunamadı!");
            }

            if (item.Durum == (int)Durum.Onaylandi || item.Durum == (int)Durum.Red)
            {
                return Ok(new { status = "success", message = "Already processed" });
            }

            var result = await _blueParaWithdrawCallbackService.ProcessWithdraw(status.Value, info.Amount, item, settings);
            return BuildResponse(result);
        }

        /// <summary>null: henüz sonuçlanmamış (bekleyen) işlem</summary>
        private static int? MapStatus(string blueParaStatus)
        {
            return blueParaStatus?.ToUpperInvariant() switch
            {
                "APPROVED" => (int)Durum.Onaylandi,
                "REJECTED" or "FAILED" or "TIMEOUT" => (int)Durum.Red,
                _ => null
            };
        }

        private static string? GetRefFromCallbackUrl(string? callbackUrl)
        {
            if (string.IsNullOrWhiteSpace(callbackUrl) || !Uri.TryCreate(callbackUrl, UriKind.Absolute, out var uri))
            {
                return null;
            }

            var query = QueryHelpers.ParseQuery(uri.Query);
            return query.TryGetValue("ref", out var value) ? value.ToString() : null;
        }

        private static string? GetRefFromMerchantTransactionId(string? merchantTransactionId)
        {
            if (string.IsNullOrWhiteSpace(merchantTransactionId))
            {
                return null;
            }

            var index = merchantTransactionId.LastIndexOf('-');
            return index > 0 ? merchantTransactionId[..index] : null;
        }

        private static bool VerifySignature(JsonElement root, string signature, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            var json = BlueParaSigner.StringifySorted(root);
            var actual = Encoding.UTF8.GetBytes(signature.Trim().ToLowerInvariant());

            // Doküman imza anahtarı için hem apiKey hem merchantKey diyor; ikisi de gizli anahtar olduğu için ikisi de kabul edilir.
            foreach (var key in new[] { settings.BlueParaApiKey, settings.BlueParaMerchantKey })
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var expected = Encoding.UTF8.GetBytes(BlueParaSigner.Sign(json, key));
                if (CryptographicOperations.FixedTimeEquals(expected, actual))
                {
                    return true;
                }
            }

            return false;
        }

        private static string? GetString(JsonElement e, string name)
            => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        private IActionResult Fail(string message)
        {
            return BuildResponse(new ProcessResultModel
            {
                IsSuccess = false,
                Status = "error",
                Message = message
            });
        }

        private IActionResult BuildResponse(ProcessResultModel result)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    status = result.Status,
                    message = result.Message
                });
            }

            return BadRequest(new
            {
                status = result.Status,
                message = result.Message
            });
        }
    }
}
