using Microsoft.AspNetCore.Mvc;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Concretes;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Models;

namespace Transactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public TransactionController(IWebSiteRepository websiteRepository,
                                     IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                     IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _websiteRepository = websiteRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(TransactionCheckRequest request)
        {
            var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetBySecretId(request.ClientId, request.ClientSecret);
            if (webSitePaymentMethodItem == null)
            {
                return NotFound(new TransactionCheckResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetStatusCheckDto(webSitePaymentMethodItem.WebSiteleriId, request.TransactionId);
            if (yatirimCekimTalepleriItem == null)
            {
                return NotFound(new TransactionCheckResponse
                {
                    Success = false,
                    Message = "Not found"
                });
            }

            TransactionCheckResponse response = new TransactionCheckResponse();
            response.Success = true;
            response.TransactionId = yatirimCekimTalepleriItem.WebSiteTransactionId;
            response.Message = GetStatusText(yatirimCekimTalepleriItem.Durum);

            return Ok(response);
        }

        private string GetStatusText(int durum)
        {
            return durum switch
            {
                (int)Durum.Bekliyor => "Pending",
                (int)Durum.Onaylandi => "Approved",
                (int)Durum.Red => "Rejected",
                _ => "Status Not Found"
            };
        }
    }
}
