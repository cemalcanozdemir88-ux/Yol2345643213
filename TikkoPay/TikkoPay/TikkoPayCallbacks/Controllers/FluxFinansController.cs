using System.Globalization;
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
    public class FluxFinansController : ControllerBase
    {
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IFluxFinansDepositCallbackService _fluxFinansDepositCallbackService;
        private readonly IFluxFinansWithdrawCallbackService _fluxFinansWithdrawCallbackService;

        public FluxFinansController(ISettingsV2Repository settingsV2Repository,
                                    IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                    IFluxFinansDepositCallbackService fluxFinansDepositCallbackService,
                                    IFluxFinansWithdrawCallbackService fluxFinansWithdrawCallbackService)
        {
            _settingsV2Repository = settingsV2Repository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _fluxFinansDepositCallbackService = fluxFinansDepositCallbackService;
            _fluxFinansWithdrawCallbackService = fluxFinansWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] FluxFinansCallbackRequest request)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var hash = GenerateHash(request.Data.TransactionId, request.Data.UserId, request.Data.Amount, settings.FluxFinansSecretKey);
            if (!hash.Equals(request.Data.Hash))
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Bu işlemi yapmaya yetkiniz yok!"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.FluxFinansTedarikciId, request.Data.TransactionId);
            if (yatirimCekimTalepleriItem is null)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Yatirim/Çekim kaydı bulunamadı!"
                });
            }

            if(yatirimCekimTalepleriItem.Durum == (int)Durum.Onaylandi)
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "İşlem daha önce onaylanmış!"
                });
            }

            if (yatirimCekimTalepleriItem.IslemTipi == (int)IslemTip.Yatirim)
            {
                var depositResult = await _fluxFinansDepositCallbackService.ProcessDeposit(request, yatirimCekimTalepleriItem, settings);

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

                var withdrawResult = await _fluxFinansWithdrawCallbackService.ProcessWithdraw(request, yatirimCekimTalepleriItem, settings);
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

        private string GenerateHash(string transactionId, string userId, string amount, string secretkey)
        {
            var normalizedAmount = decimal
                .Parse(amount.Trim().Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
                .ToString("F2", CultureInfo.InvariantCulture);

            var payload = $"{transactionId}{userId}{amount}{secretkey}";

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
