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
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly IBankaKodlariRepository _bankaKodlariRepository;

        public WithdrawController(IGeneralHelper generalHelper,
                                  IWebSiteRepository websiteRepository,
                                  IWithdrawRepository withdrawRepository,
                                  ISettingsV2Repository settingsV2Repository,
                                  IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository,
                                  IBankaKodlariRepository bankaKodlariRepository)
        {
            _generalHelper = generalHelper;
            _websiteRepository = websiteRepository;
            _withdrawRepository = withdrawRepository;
            _settingsV2Repository = settingsV2Repository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
            _bankaKodlariRepository = bankaKodlariRepository;
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

            var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetBySecretId(request.ClientId, request.ClientSecret);
            if (webSitePaymentMethodItem == null)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var webSiteBakiye = await _websiteRepository.GetWebsiteBakiyeAsync(webSitePaymentMethodItem.WebSiteleriId);
            if (webSiteBakiye < webSitePaymentMethodItem.BakiyeLimitor)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Website balance is insufficient for withdrawal"
                });
            }

            var remainingBakiye = webSiteBakiye - ((decimal)request.Amount);
            if (remainingBakiye < webSitePaymentMethodItem.BakiyeLimitor)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "The website balance hit the withdrawal limit."
                });
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            request.Iban = request.Iban.Replace(" ", "").Trim();
            var ibanCheckResult = CheckIfIbanBanned(request.Iban, settings.IbanYasakliListesi);
            if (ibanCheckResult)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Iban Banned"
                });
            }

            if (((decimal)request.Amount) < webSitePaymentMethodItem.MinCekimTutari)
            {
                return NotFound(new WithdrawCreateResponse
                {
                    Success = false,
                    Message = "Invalid Amount"
                });
            }

            var bankName = await GetBankNameFromIban(request.Iban);

            WithdrawCreateResponse responseModel = new WithdrawCreateResponse();
            _withdrawRepository.CreateWithdraw(request, webSitePaymentMethodItem, null, bankName);

            responseModel.Success = true;
            responseModel.Message = "Withdraw request was successfully processed.";

            return Ok(responseModel);
        }

        private async Task<string> GetBankNameFromIban(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return "Geçersiz IBAN";

            iban = iban.Replace(" ", "").Trim().ToUpper();

            if (!iban.StartsWith("TR") || iban.Length != 26)
                return "Geçersiz IBAN";

            string bankCode = iban.Substring(5, 4);

            var bankaKodlariItem = await _bankaKodlariRepository.GetByKodAsync(bankCode);
            if (bankaKodlariItem is null)
                return "Banka Bulunamadı";

            return bankaKodlariItem.Baslik;
        }

        private bool CheckIfIbanBanned(string iban, string ibanList)
        {
            if (string.IsNullOrWhiteSpace(iban) || string.IsNullOrWhiteSpace(ibanList))
                return false;

            string normalizedIban = NormalizeIban(iban);
            string[] ibansInList = ibanList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string listIban in ibansInList)
            {
                if (normalizedIban == NormalizeIban(listIban))
                    return true;
            }

            return false;
        }

        private string NormalizeIban(string iban)
        {
            return iban?.Replace(" ", "").ToUpper() ?? string.Empty;
        }

        private bool CheckHash(WithdrawCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }
    }
}
