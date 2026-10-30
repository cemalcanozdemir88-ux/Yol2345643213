using Microsoft.AspNetCore.Mvc;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Models;

namespace Transactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly IWithdrawRepository _withdrawRepository;

        public WithdrawController(IGeneralHelper generalHelper, IWebSiteRepository websiteRepository, IWithdrawRepository withdrawRepository)
        {
            _generalHelper = generalHelper;
            _websiteRepository = websiteRepository;
            _withdrawRepository = withdrawRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(WithdrawCreateRequest request)
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
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            _withdrawRepository.CreateWithdraw(request, webSiteItem);

            WithdrawCreateResponse responseModel = new WithdrawCreateResponse();
            responseModel.Success = true;
            responseModel.Message = "Withdraw request was successfully processed.";

            return Ok(responseModel);
        }

        private bool CheckHash(WithdrawCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }
    }
}
