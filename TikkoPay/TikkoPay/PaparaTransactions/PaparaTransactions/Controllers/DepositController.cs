using Microsoft.AspNetCore.Mvc;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Models;

namespace Transactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGeneralHelper _generalHelper;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IDepositRepository _depositRepository;

        public DepositController(IConfiguration configuration, IGeneralHelper generalHelper, IWebSiteRepository websiteRepository, IDepositRepository depositRepository)
        {
            _configuration = configuration;
            _generalHelper = generalHelper;
            _websiteRepository = websiteRepository;
            _depositRepository = depositRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepositCreateRequest request)
        {
            if (!CheckHash(request))
            {
                return NotFound(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var webSiteItem = await _websiteRepository.GetWebSiteItem(request.ClientId, request.ClientSecret);
            if (webSiteItem == null)
            {
                return NotFound(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var depositTransactionId = await CreateDepositTransactionItem(request, webSiteItem.Id);
            DepositCreateResponse responseModel = new()
            {
                Success = true,
                Message = "The payment form was created successfully.",
                PaymentFrameUrl = $"{_configuration["WebSiteUrl"]}?transaction={depositTransactionId}"
            };

            return Ok(responseModel);
        }

        private async Task<Guid> CreateDepositTransactionItem(DepositCreateRequest request, Guid webSiteId)
        {
            var waitingDepositTransaction = await _depositRepository.GetWaitingDepositTransaction(webSiteId, request.UserId);
            if (waitingDepositTransaction != null)
            {
                TimeSpan gecenSure = DateTime.Now - waitingDepositTransaction.KayitTarihi;
                if (gecenSure.TotalMinutes <= GetValidTransactionMinute())
                {
                    return waitingDepositTransaction.Id;
                }
            }

            var depositTransactionId = _depositRepository.Insert(request, webSiteId);
            return depositTransactionId;
        }

        private bool CheckHash(DepositCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }

        private int GetValidTransactionMinute()
        {
            return _configuration.GetValue<int>("ValidTransactionMinute");
        }
    }
}
