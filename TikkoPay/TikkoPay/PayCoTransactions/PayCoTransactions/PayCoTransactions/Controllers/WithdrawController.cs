using Microsoft.AspNetCore.Mvc;
using PayCoTransactions.Infrastructure.Helpers;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Infrastructure.Services.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IWithdrawTransactionRepository _withdrawRepository;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public WithdrawController(IGeneralHelper generalHelper,
                                  IWebSiteRepository websiteRepository,
                                  IWithdrawTransactionRepository withdrawRepository,
                                  IConfiguration configuration,
                                  IPaymentService paymentService)
        {
            _generalHelper = generalHelper;
            _websiteRepository = websiteRepository;
            _withdrawRepository = withdrawRepository;
            _configuration = configuration;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(WithdrawCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PaycoWalletId))
            {
                return BadRequest(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "PaycoWalletId can not be empty"
                });
            }

            if (request.Amount == 0)
            {
                return BadRequest(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Amount can not be 0"
                });
            }

            if (!CheckHash(request))
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var webSiteItem = await _websiteRepository.GetWebSiteItem(request.ClientId, request.ClientSecret);
            if (webSiteItem == null)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            if (string.IsNullOrWhiteSpace(request.PaycoWalletFullName))
            {
                request.PaycoWalletFullName = request.FullName;
            }

            _withdrawRepository.CreateWithdraw(request, webSiteItem);

            WithdrawCreateResponse responseModel = new WithdrawCreateResponse();

            var paycoWithDrawResponse = await MakePaycoWithdraw(request);
            if (paycoWithDrawResponse.status.Equals("ERR"))
            {
                responseModel.Success = false;
                responseModel.Message = $"Error: {paycoWithDrawResponse.message}";
            }
            else
            {
                responseModel.Success = true;
                responseModel.Message = "Withdraw request was successfully processed.";
            }

            return Ok(responseModel);
        }

        private bool CheckHash(WithdrawCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }

        private async Task<PaycoWithDrawResponse> MakePaycoWithdraw(WithdrawCreateRequest request)
        {
            var secret = _configuration.GetValue<string>("SecretKey");
            var apiUrl = _configuration.GetValue<string>("WithdrawUrl");

            var withdrawRequest = new PaycoWithDrawRequest
            {
                secret = secret,
                player_username = request.UserName,
                player_ceppay_wallet_id = request.PaycoWalletId,
                player_ceppay_wallet_fullname = request.PaycoWalletFullName,
                amount = request.Amount.ToString(),
                tx_id = request.TransactionId
            };

            var result = await _paymentService.MakeWithdrawAsync(apiUrl, withdrawRequest);
            return result;
        }
    }
}
