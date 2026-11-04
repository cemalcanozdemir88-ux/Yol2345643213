using System.Security.Cryptography;
using System.Text;
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
    public class ViPayController : ControllerBase
    {
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IViPayDepositCallbackService _viPayDepositCallbackService;
        private readonly IViPayWithdrawCallbackService _viPayWithdrawCallbackService;

        public ViPayController(ISettingsV2Repository settingsV2Repository,
                               IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                               IViPayDepositCallbackService viPayDepositCallbackService,
                               IViPayWithdrawCallbackService viPayWithdrawCallbackService)
        {
            _settingsV2Repository = settingsV2Repository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _viPayDepositCallbackService = viPayDepositCallbackService;
            _viPayWithdrawCallbackService = viPayWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] ViPayCallbackRequest request)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var isValid = VerifyCallback(request.Ref, request.Status, request.Amount, request.Signature, settings.ViPaySignKey);
            if (!isValid)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Bu işlemi yapmaya yetkiniz yok!"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.ViPayTedarikciId, request.Ref)
                                            ?? await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.ViPayTedarikciId2, request.Ref);
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
                var depositResult = await _viPayDepositCallbackService.ProcessDeposit(request, yatirimCekimTalepleriItem, settings);

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

                var withdrawResult = await _viPayWithdrawCallbackService.ProcessWithdraw(request, yatirimCekimTalepleriItem, settings);
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

        private static bool VerifyCallback(string reference, string status, string amount, string? signature, string signingSecret)
        {
            var message = string.Concat(reference, status, amount);

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingSecret));
            var expected = Convert.ToHexString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(message)))
                .ToLowerInvariant();

            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var actualBytes = Encoding.UTF8.GetBytes(signature ?? string.Empty);

            return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
        }
    }
}
