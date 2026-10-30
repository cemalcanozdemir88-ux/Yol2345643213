using Microsoft.AspNetCore.Mvc;
using PayCoTransactions.Infrastructure.Helpers;
using PayCoTransactions.Infrastructure.Repositories.Interfaces;
using PayCoTransactions.Infrastructure.Services.Interfaces;
using PayCoTransactions.Models;

namespace PayCoTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IConfiguration _configuration;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IPaymentService _paymentService;
        private readonly IDepositTransactionRepository _depositRepository;

        public DepositController(IGeneralHelper generalHelper,
                                 IConfiguration configuration,
                                 IWebSiteRepository websiteRepository,
                                 IPaymentService paymentService,
                                 IDepositTransactionRepository depositRepository)
        {
            _generalHelper = generalHelper;
            _configuration = configuration;
            _websiteRepository = websiteRepository;
            _paymentService = paymentService;
            _depositRepository = depositRepository;
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

            decimal validAmount = _configuration.GetValue<decimal>("MinimumDepositValue");
            _ = decimal.TryParse(request.Amount, out decimal amount);
            if (amount < validAmount)
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

            var paycoStartDepositResponse = await PayCoStartDepositAsync(request);
            if (paycoStartDepositResponse.status.Equals("ERR"))
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "PaymentFrameUrl Create Error"
                });
            }

            var depositTransactionId = CreateDepositTransactionItem(request, webSiteItem.Id);

            var responseModel = new DepositCreateResponse
            {
                Success = true,
                Message = "The payment form was created successfully.",
                PaymentFrameUrl = paycoStartDepositResponse.redirectUrl
            };

            return Ok(responseModel);
        }

        private Guid CreateDepositTransactionItem(DepositCreateRequest request, Guid webSiteId)
        {
            var depositTransactionId = _depositRepository.Insert(request, webSiteId);
            return depositTransactionId;
        }

        private bool CheckHash(DepositCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }

        private async Task<PaycoStartDepositResponse> PayCoStartDepositAsync(DepositCreateRequest request)
        {
            var secret = _configuration.GetValue<string>("SecretKey");
            var depositUrl = _configuration.GetValue<string>("DepositUrl");

            var depositRequest = new PaycoStartDepositRequest
            {
                secret = secret,
                amount = request.Amount,
                player_username = request.UserName,
                tx_id = request.TransactionId,
                player_id = request.UserId,
                player_fullname = request.FullName
            };

            var result = await _paymentService.StartDepositAsync(depositUrl, depositRequest);
            return result;
        }
    }
}
