using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ParaYolCallbacks.Infrastructure.Dtos;
using ParaYolCallbacks.Infrastructure.Helpers;
using ParaYolCallbacks.Infrastructure.Repositories.Interfaces;
using ParaYolCallbacks.Infrastructure.Services.Interfaces;
using ParaYolCallbacks.Models;

namespace ParaYolCallbacks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViPayController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IViPayDepositCallbackService _viPayDepositCallbackService;
        private readonly IViPayWithdrawCallbackService _viPayWithdrawCallbackService;

        public ViPayController(ISettingsRepository settingsRepository,
                               IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                               IViPayDepositCallbackService viPayDepositCallbackService,
                               IViPayWithdrawCallbackService viPayWithdrawCallbackService)
        {
            _settingsRepository = settingsRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _viPayDepositCallbackService = viPayDepositCallbackService;
            _viPayWithdrawCallbackService = viPayWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] ViPayCallbackRequest request)
        {
            var settings = await _settingsRepository.GetFirst();

            var hash = GenerateHash(request.Ref, request.Status, request.Amount, settings.ViPaySignKey);
            if (!hash.Equals(request.Signature))
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Bu işlemi yapmaya yetkiniz yok!"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.ViPayTedarikciId, request.Ref);
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

        private static string GenerateHash(string reference, string status, string amount,string signingSecret)
        {
            string message = $"{reference}{status}{amount}";

            byte[] key = Encoding.UTF8.GetBytes(signingSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(key);

            byte[] hash = hmac.ComputeHash(messageBytes);

           return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
