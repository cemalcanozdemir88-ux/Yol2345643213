using Microsoft.AspNetCore.Mvc;
using Transactions.Infrastructure.Dtos;
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
        private readonly IKullaniciBlackListsRepository _kullaniciBlackListsRepository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public DepositController(IConfiguration configuration,
                                 IGeneralHelper generalHelper,
                                 IWebSiteRepository websiteRepository,
                                 IDepositRepository depositRepository,
                                 IKullaniciBlackListsRepository kullaniciBlackListsRepository,
                                 IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _configuration = configuration;
            _generalHelper = generalHelper;
            _websiteRepository = websiteRepository;
            _depositRepository = depositRepository;
            _kullaniciBlackListsRepository = kullaniciBlackListsRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
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

            var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetBySecretId(request.ClientId, request.ClientSecret);
            if (webSitePaymentMethodItem == null)
            {
                return NotFound(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var isUserInBlackList = await _kullaniciBlackListsRepository.IsUserInBlacklistAsync(webSitePaymentMethodItem.WebSiteleriId, request.UserId);
            if (isUserInBlackList)
            {
                return NotFound(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid User"
                });
            }

            _ = decimal.TryParse(request.Amount, out var amount);
            if (amount < webSitePaymentMethodItem.MinYatirimTutari)
            {
                return NotFound(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid Amount"
                });
            }

            var depositTransactionId = await CreateDepositTransactionItem(request, webSitePaymentMethodItem);
            DepositCreateResponse responseModel = new()
            {
                Success = true,
                Message = "The payment form was created successfully.",
                PaymentFrameUrl = $"{_configuration["WebSiteUrl"]}?transaction={depositTransactionId}"
            };

            return Ok(responseModel);
        }

        private async Task<Guid> CreateDepositTransactionItem(DepositCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto)
        {
            var waitingDepositTransaction = await _depositRepository.GetWaitingDepositTransaction(webSitePaymentMethodDto.WebSiteleriId, request.UserId);
            if (waitingDepositTransaction != null)
            {
                TimeSpan gecenSure = DateTime.Now - waitingDepositTransaction.KayitTarihi;
                if (gecenSure.TotalMinutes <= GetValidTransactionMinute())
                {
                    return waitingDepositTransaction.Id;
                }
            }

            var depositTransactionId = _depositRepository.Insert(request, webSitePaymentMethodDto);
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
