using Microsoft.AspNetCore.Mvc;
using PayFixTransactions.Infrastructure.Helpers;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Infrastructure.Services.Interfaces;
using PayFixTransactions.Models;

namespace PayFixTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IPaymentService _paymentService;
        private readonly IDepositTransactionRepository _depositRepository;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IConfiguration _configuration;

        public DepositController(IGeneralHelper generalHelper, 
                                 IPaymentService paymentService, 
                                 IDepositTransactionRepository depositRepository, 
                                 IWebSiteRepository websiteRepository, 
                                 IConfiguration configuration)
        {
            _generalHelper = generalHelper;
            _paymentService = paymentService;
            _depositRepository = depositRepository;
            _websiteRepository = websiteRepository;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepositCreateRequest request)
        {
            if (!CheckHash(request))
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            decimal validAmount = _configuration.GetValue<decimal>("PayFixMinimumDepositValue");
            _ = decimal.TryParse(request.Amount, out decimal amount);
            if(amount < validAmount)
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = $"Minimum yatırım tutarı {validAmount} TL'dir."
                });
            }

            var webSiteItem = await _websiteRepository.GetWebSiteItem(request.ClientId, request.ClientSecret);
            if (webSiteItem == null)
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var provisionResponse = await MakeProvisionAsync(request);
            if (provisionResponse.Status.Equals("ERR"))
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "PaymentFrameUrl Create Error"
                });
            }

            var depositTransactionId = CreateDepositTransactionItem(request, webSiteItem.Id);

            var  responseModel = new DepositCreateResponse
            {
                Success = true,
                Message = "The payment form was created successfully.",
                PaymentFrameUrl = provisionResponse.RedirectUrl
            };

            return Ok(responseModel);
        }

        private Guid CreateDepositTransactionItem(DepositCreateRequest request, Guid webSiteId)
        {
            var depositTransactionId = _depositRepository.Insert(request, webSiteId);
            return depositTransactionId;
        }

        private async Task<ProvisionResponse> MakeProvisionAsync(DepositCreateRequest request)
        {
            var secret = _configuration.GetValue<string>("PayFixSecret");
            var provisionUrl = _configuration.GetValue<string>("ProvisionUrl");

            var provisionRequest = new ProvisionRequest()
            {
                secret = secret,
                amount = request.Amount,
                enduser_userId = request.UserId,
                enduser_username = request.UserName,
                enduser_fullname = request.FullName,
                customer_transaction_id = request.TransactionId,
                payment_short_code = "fix"
            };

            var result = await _paymentService.MakeProvisionAsync(provisionUrl, provisionRequest);
            return result;
        }

        private bool CheckHash(DepositCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }
    }
}
