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
    public class ParoPayController : ControllerBase
    {
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IParoPayDepositCallbackService _paroPayDepositCallbackService;
        private readonly IParoPayWithdrawCallbackService _paroPayWithdrawCallbackService;

        public ParoPayController(ISettingsV2Repository settingsV2Repository,
                                 IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                 IParoPayDepositCallbackService paroPayDepositCallbackService,
                                 IParoPayWithdrawCallbackService paroPayWithdrawCallbackService)
        {
            _settingsV2Repository = settingsV2Repository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _paroPayDepositCallbackService = paroPayDepositCallbackService;
            _paroPayWithdrawCallbackService = paroPayWithdrawCallbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] ParoPayCallbackRequest request)
        {
            var settings = await _settingsV2Repository.GetSettingsAsync();

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetByWebSiteTransactionId(settings.ParoPayTedarikciId, request.TransactionId);
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
                var hash = CreateMD5Hash($"pay{request.UserId}{request.TransactionId}{request.Amount}{settings.ParoPayApiSecret}");
                if (!hash.Equals(request.Hash))
                {
                    return BuildResponse(new ProcessResultModel
                    {
                        IsSuccess = false,
                        Message = "Bu işlemi yapmaya yetkiniz yok!"
                    });
                }

                var depositResult = await _paroPayDepositCallbackService.ProcessDeposit(request, yatirimCekimTalepleriItem, settings);

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

                var hash = CreateMD5Hash($"withdraw{request.UserId}{request.TransactionId}{request.Amount}{yatirimCekimTalepleriItem.HesapNo}Fast{yatirimCekimTalepleriItem.HesapSahibi}{settings.ParoPayApiSecret}");
                if (!hash.Equals(request.Hash))
                {
                    return BuildResponse(new ProcessResultModel
                    {
                        IsSuccess = false,
                        Message = "Bu işlemi yapmaya yetkiniz yok!"
                    });
                }

                var withdrawResult = await _paroPayWithdrawCallbackService.ProcessWithdraw(request, yatirimCekimTalepleriItem, settings);
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

        private string CreateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
