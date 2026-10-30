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
    public class NhController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly INhDepositCallbackService _nhDepositCallbackService;
        private readonly INhWithdrawCallbackService _nhWithdrawCallbackService;

        public NhController(ISettingsRepository settingsRepository,
                            IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                            INhDepositCallbackService nhDepositCallbackService,
                            INhWithdrawCallbackService nhWithdrawCallbackService)
        {
            _settingsRepository = settingsRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _nhDepositCallbackService = nhDepositCallbackService;
            _nhWithdrawCallbackService = nhWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] NhCallbackRequest request)
        {
            var settings = await _settingsRepository.GetFirst();

            var hash = GenerateHash(request.transaction_id, request.userID, request.status, request.type);
            if (!hash.Equals(request.hash))
            {
                return BuildResponse(new ProcessResultModel
                {
                    IsSuccess = false,
                    Message = "Bu işlemi yapmaya yetkiniz yok!"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.NhTedarikciId, request.transaction_id);
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
                var depositResult = await _nhDepositCallbackService.ProcessDeposit(request, yatirimCekimTalepleriItem, settings);

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

                var withdrawResult = await _nhWithdrawCallbackService.ProcessWithdraw(request, yatirimCekimTalepleriItem, settings);
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

        private static string GenerateHash(string TransactionId, string UserId, int Status, string Type)
        {
            using var sha1 = SHA1.Create();
            var hashInput = $"{TransactionId}{UserId}{Status}{Type}";
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(hashInput));

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
