using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TikkoPayCallbacks.Infrastructure.Dtos;
using TikkoPayCallbacks.Infrastructure.Helpers;
using TikkoPayCallbacks.Infrastructure.Repositories.Interfaces;
using TikkoPayCallbacks.Infrastructure.Services.Interfaces;
using TikkoPayCallbacks.Models;

namespace TikkoPayCallbacks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixHavaleController : ControllerBase
    {
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IFixHavaleDepositCallbackService _fixHavaleDepositCallbackService;
        private readonly IFixHavaleWithdrawCallbackService _fixHavaleWithdrawCallbackService;

        public FixHavaleController(ISettingsV2Repository settingsV2Repository,
                                   IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                   IFixHavaleDepositCallbackService fixHavaleDepositCallbackService,
                                   IFixHavaleWithdrawCallbackService fixHavaleWithdrawCallbackService)
        {
            _settingsV2Repository = settingsV2Repository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _fixHavaleDepositCallbackService = fixHavaleDepositCallbackService;
            _fixHavaleWithdrawCallbackService = fixHavaleWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback(CancellationToken cancellationToken)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync(cancellationToken);

            var timestamp = Request.Headers["X-Timestamp"].ToString();
            var eventId = Request.Headers["X-Event-Id"].ToString();
            var signature = Request.Headers["X-Signature"].ToString();

            var verified = VerifyWebhook(rawBody, timestamp, signature, settings.FixHavaleWebhookSecret);

            if (string.IsNullOrWhiteSpace(eventId) || !verified)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Invalid webhook signature"
                });
            }

            var payload = JsonSerializer.Deserialize<FixHavaleCallbackRequest>(rawBody);

            if (payload is null)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Invalid JSON"
                });
            }

            var transaction = payload.Transaction;
            if (transaction is null || string.IsNullOrWhiteSpace(transaction.Id))
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Invalid transaction"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.FixHavaleTedarikciId, transaction.Id);
            if (yatirimCekimTalepleriItem is null)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Yatirim/Çekim kaydı bulunamadı!"
                });
            }

            if (yatirimCekimTalepleriItem.Durum == (int)Durum.Onaylandi)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "İşlem daha önce onaylanmış!"
                });
            }

            if (yatirimCekimTalepleriItem.IslemTipi == (int)IslemTip.Yatirim)
            {
                var depositResult = await _fixHavaleDepositCallbackService.ProcessDeposit(payload, yatirimCekimTalepleriItem, settings);

                return BuildResponse(depositResult);
            }
            else if (yatirimCekimTalepleriItem.IslemTipi == (int)IslemTip.Cekim)
            {
                if (yatirimCekimTalepleriItem.Durum == (int)Durum.Red)
                {
                    return BuildResponse(new ProcessResultModel
                    {
                        IsSuccess = false,
                        Message = "Çekim işlemi daha önce red edilmiş!"
                    });
                }

                var withdrawResult = await _fixHavaleWithdrawCallbackService.ProcessWithdraw(payload, yatirimCekimTalepleriItem, settings);
                return BuildResponse(withdrawResult);
            }
            else
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "İşlem tipi bulunamadı!"
                });
            }
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

        private bool VerifyWebhook(string rawBody, string timestamp, string signature, string fixHavaleWebhookSecret)
        {
            if (string.IsNullOrWhiteSpace(timestamp)
                || !long.TryParse(timestamp, out var milliseconds)
                || string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            if (Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - milliseconds / 1000) > 300)
            {
                return false;
            }

            var expected = Sign($"{timestamp}.{rawBody}", fixHavaleWebhookSecret);
            var actual = signature.StartsWith("sha256=", StringComparison.Ordinal) ? signature[7..] : signature;

            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));
        }

        private static string Sign(string value, string secret)
        => Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }
}
