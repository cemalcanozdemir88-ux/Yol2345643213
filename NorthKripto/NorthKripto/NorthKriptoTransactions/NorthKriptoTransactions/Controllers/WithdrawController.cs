using Microsoft.AspNetCore.Mvc;
using NorthKriptoTransactions.Infrastructure.Helpers;
using NorthKriptoTransactions.Infrastructure.Repositories.Interfaces;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly IGeneralHelper _generalHelper;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly IWebSiteRepository _websiteRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IWithdrawRepository _withdrawRepository;

        public WithdrawController(IGeneralHelper generalHelper,
                                  IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository,
                                  IWebSiteRepository websiteRepository,
                                  ISettingsRepository settingsRepository,
                                  IWithdrawRepository withdrawRepository)
        {
            _generalHelper = generalHelper;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
            _websiteRepository = websiteRepository;
            _settingsRepository = settingsRepository;
            _withdrawRepository = withdrawRepository;
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

            if (!request.WalletAddressType.Equals("USDT_TRC20"))
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "We support only Usdt TRC20"
                });
            }

            var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetBySecretId(request.ClientId, request.ClientSecret);
            if (webSitePaymentMethodItem == null)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            //var webSiteBakiye = await _websiteRepository.GetWebsiteBakiyeAsync(webSitePaymentMethodItem.WebSiteleriId);
            //if (webSiteBakiye < webSitePaymentMethodItem.BakiyeLimitor)
            //{
            //    return NotFound(new WithdrawCreateResponse
            //    {
            //        Success = false,
            //        Message = "Website balance is insufficient for withdrawal"
            //    });
            //}

            var settings = await _settingsRepository.GetFirst();
            var ibanCheckResult = CheckIfWalletIsBanned(request.WalletAddress, settings.CuzdanYasakliListesi);
            if (ibanCheckResult)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Iban Banned"
                });
            }

            float minCekimTutari = 1000;
            if (request.Amount < minCekimTutari)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid Amount"
                });
            }

            WithdrawCreateResponse responseModel = new WithdrawCreateResponse();
            _withdrawRepository.CreateWithdraw(request, webSitePaymentMethodItem);

            responseModel.Success = true;
            responseModel.Message = "Withdraw request was successfully processed.";

            return Ok(responseModel);
        }

        private bool CheckIfWalletIsBanned(string wallet, string walletList)
        {
            if (string.IsNullOrWhiteSpace(wallet) || string.IsNullOrWhiteSpace(walletList))
                return false;

            string normalizedIban = NormalizeWallet(wallet);
            string[] walletsInList = walletList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string listIban in walletsInList)
            {
                if (normalizedIban == NormalizeWallet(listIban))
                    return true;
            }

            return false;
        }

        private string NormalizeWallet(string wallet)
        {
            return wallet?.Replace(" ", "").ToUpper() ?? string.Empty;
        }

        private bool CheckHash(WithdrawCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }
    }
}
