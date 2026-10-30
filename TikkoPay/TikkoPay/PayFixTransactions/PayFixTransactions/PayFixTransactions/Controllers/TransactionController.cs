using Microsoft.AspNetCore.Mvc;
using PayFixTransactions.Infrastructure.Repositories.Interfaces;
using PayFixTransactions.Models;

namespace PayFixTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;
        private readonly ICallbackRepository _callbackRepository;

        public TransactionController(IWebSiteRepository websiteRepository, 
            IYatirimTalepleriRepository yatirimTalepleriRepository, 
            ICallbackRepository callbackRepository)
        {
            _websiteRepository = websiteRepository;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
            _callbackRepository = callbackRepository;
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check(TransactionCheckRequest request)
        {
            var webSiteItem = await _websiteRepository.GetWebSiteItem(request.ClientId, request.ClientSecret);
            if (webSiteItem == null)
            {
                return NotFound(new TransactionCheckResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var callBackRequestItem = await _callbackRepository.GetByCustomerTransactionId(request.TransactionId);
            if (callBackRequestItem == null)
            {
                return NotFound(new TransactionCheckResponse
                {
                    Success = false,
                    Message = "Transaction not found"
                });
            }

            if (callBackRequestItem.Type.Equals("deposit"))
            {
                var yatirimTalepleriItem = await _yatirimTalepleriRepository.GetStatusCheckDto(webSiteItem.Id, request.TransactionId);
                if (yatirimTalepleriItem == null)
                {
                    return NotFound(new TransactionCheckResponse
                    {
                        Success = false,
                        Message = "Not found"
                    });
                }

                TransactionCheckResponse response = new TransactionCheckResponse();
                response.Success = true;
                response.TransactionId = yatirimTalepleriItem.WebSiteTransactionId;
                response.Message = "Approved";

                return Ok(response);
            }
            else
            {
                TransactionCheckResponse response = new TransactionCheckResponse();
                response.Success = true;
                response.TransactionId = callBackRequestItem.CustomerTransactionId;

                if (callBackRequestItem.Status.Equals("completed"))
                {
                    response.Message = "Approved";
                }
                else if (callBackRequestItem.Status.Equals("canceled"))
                {
                    response.Message = "Rejected";
                }

                return Ok(response);
            }

            
        }
    }
}
