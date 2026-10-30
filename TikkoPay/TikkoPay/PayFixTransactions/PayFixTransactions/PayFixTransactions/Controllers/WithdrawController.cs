using Microsoft.AspNetCore.Mvc;
using PayFixTransactions.Infrastructure.Helpers;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Infrastructure.Services.Interfaces;
using PayFixTransactions.Models;
using System.Text;

namespace PayFixTransactions.Controllers
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

            if (string.IsNullOrWhiteSpace(request.PayfixFullName))
            {
                request.PayfixFullName = request.FullName;
            }

            _withdrawRepository.CreateWithdraw(request, webSiteItem);

            WithdrawCreateResponse responseModel = new WithdrawCreateResponse();

            var payfixWithDrawResponse = await MakePayfixWithdraw(request);
            if (payfixWithDrawResponse.status.Equals("ERR"))
            {
                responseModel.Success = false;

                if (payfixWithDrawResponse.errors != null && payfixWithDrawResponse.errors.Any())
                {
                    StringBuilder sbErrors = new StringBuilder();
                    foreach (var item in payfixWithDrawResponse.errors)
                    {
                        sbErrors.Append(item.message);
                    }

                    responseModel.Message = sbErrors.ToString();
                }
                else
                {
                    responseModel.Message = payfixWithDrawResponse.message;
                }
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

        private async Task<PayfixWithDrawResponse> MakePayfixWithdraw(WithdrawCreateRequest request)
        {
            var secret = _configuration.GetValue<string>("PayFixSecret");
            var apiUrl = _configuration.GetValue<string>("WithdrawUrl");

            var withdrawRequest = new PayfixWithDrawRequest
            {
                secret = secret,
                transaction_id = request.TransactionId,
                amount = request.Amount.ToString(),
                enduser_username = request.UserName,
                enduser_remote_id = request.PayfixId,
                enduser_remote_fullname = request.PayfixFullName,
                payment_short_code = "fix",
                enduser_userId = request.UserId
            };

            var result = await _paymentService.MakeWithdrawAsync(apiUrl, withdrawRequest);
            return result;
        }
    }
}
