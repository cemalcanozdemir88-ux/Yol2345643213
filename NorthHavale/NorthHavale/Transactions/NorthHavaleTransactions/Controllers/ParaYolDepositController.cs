using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Helpers;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Infrastructure.Services.Interfaces;
using NorthHavaleTransactions.Models;

namespace NorthHavaleTransactions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParaYolDepositController : ControllerBase
    {
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;
        private readonly IGeneralHelper _generalHelper;
        private readonly IKullaniciBlackListsRepository _kullaniciBlackListsRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IAvailableBankAccountService _availableBankAccountService;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly ITedarikcilerRepository _tedarikcilerRepository;

        public ParaYolDepositController(IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository,
                                        IGeneralHelper generalHelper,
                                        IKullaniciBlackListsRepository kullaniciBlackListsRepository,
                                        IDepositRepository depositRepository,
                                        IWebSiteRepository webSiteRepository,
                                        IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                                        IAvailableBankAccountService availableBankAccountService,
                                        IHavaleBankalariRepository havaleBankalariRepository,
                                        ITedarikcilerRepository tedarikcilerRepository)
        {
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
            _generalHelper = generalHelper;
            _kullaniciBlackListsRepository = kullaniciBlackListsRepository;
            _depositRepository = depositRepository;
            _webSiteRepository = webSiteRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _availableBankAccountService = availableBankAccountService;
            _havaleBankalariRepository = havaleBankalariRepository;
            _tedarikcilerRepository = tedarikcilerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ParaYolDepositCreateRequest request)
        {
            var webSitePaymentMethodItem = await _webSitePaymentMethodsRepository.GetByClientId(request.ClientId);
            if (webSitePaymentMethodItem == null)
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId"
                });
            }

            if (string.IsNullOrEmpty(request.ClientSecret))
            {
                request.ClientSecret = webSitePaymentMethodItem.ApiClientSecret;
            }

            if (!CheckHash(request))
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid ClientId or ClientSecret"
                });
            }

            var isUserInBlackList = await _kullaniciBlackListsRepository.IsUserInBlacklistAsync(webSitePaymentMethodItem.WebSiteleriId, request.UserId);
            if (isUserInBlackList)
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid User"
                });
            }

            if (request.Amount < webSitePaymentMethodItem.MinYatirimTutari)
            {
                return BadRequest(new DepositCreateResponse
                {
                    Success = false,
                    Message = "Invalid Amount"
                });
            }

            var depositTransactionId = CreateDepositTransactionItem(request, webSitePaymentMethodItem);

            var depositTransaction = await _depositRepository.GetById(depositTransactionId);

            var webSiteItem = await _webSiteRepository.GetByIdAsync(depositTransaction.WebSiteId);
            if (webSiteItem == null)
            {
                return BadRequest(new DepositCreateResponse { Message = "WebSite Bulunamadı!", Success = false });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetCountByWebSiteTransactionId(depositTransaction.TransactionId);
            if (yatirimCekimTalepleriItem > 0)
            {
                return BadRequest(new DepositCreateResponse { Message = "Geçersiz transaction!", Success = false });
            }

            _ = decimal.TryParse(depositTransaction.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount);

            var pendingYatirimTalepItem = await _yatirimCekimTalepleriRepository.GetPendingYatirimTalepleri(depositTransaction.TransactionId);
            if (pendingYatirimTalepItem != null)
            {
                var response = new NhBankAccountResponse
                {
                    success = true,
                    message = string.Empty,
                    data = new NhFastDepositData
                    {
                        payment_id = "",
                        account_holder = pendingYatirimTalepItem.HesapSahibi,
                        bank = "Fast",
                        iban = pendingYatirimTalepItem.HesapNo,
                        reference = depositTransaction.TransactionId,
                        amount = pendingYatirimTalepItem.Tutar,
                        currency = "TRY",
                        member_id = pendingYatirimTalepItem.UserId
                    }
                };

                return Ok(response);
            }

            var havaleBankalariItem = await _availableBankAccountService.GetValidBankAccountAsync(depositTransaction, webSiteItem, amount, string.Empty);
            if (havaleBankalariItem == null)
            {
                return BadRequest(new DepositCreateResponse { Message = "Hesap bulunamadı!", Success = false });
            }

            await SendTransaction(depositTransaction, havaleBankalariItem);

            return Ok(new NhBankAccountResponse
            {
                success = true,
                message = string.Empty,
                data = new NhFastDepositData
                {
                    payment_id = "",
                    account_holder = havaleBankalariItem.HesapSahibi,
                    bank = "Fast",
                    iban = havaleBankalariItem.HesapNo,
                    reference = depositTransaction.TransactionId,
                    amount = amount,
                    currency = "TRY",
                    member_id = havaleBankalariItem.UserId
                }
            });
        }

        private long? ParseToLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (long.TryParse(value, out long result))
            {
                return result;
            }

            return null;
        }

        private async Task<string> SendTransaction(DepositTransaction depositTransactionItem, AvailableHavaleBankItemDto availableHavaleBankItemDto)
        {
            var havaleBankalariItem = await _havaleBankalariRepository.GetById(availableHavaleBankItemDto.Id);
            var webSiteItem = await _webSiteRepository.GetByIdAsync(depositTransactionItem.WebSiteId);

            var item = new YatirimCekimTalepleri();
            item.Id = Guid.NewGuid();
            item.TalepNo = DateTime.Now.ToString("yyyyMMdd");
            item.WebSitePaymentMethodId = depositTransactionItem.WebSitePaymentMethodId;
            item.TedarikciId = havaleBankalariItem.TedarikciId;
            item.HavaleBankaId = havaleBankalariItem.Id;
            item.BankaId = havaleBankalariItem.BankaId;
            item.WebSiteId = depositTransactionItem.WebSiteId;
            item.WebSiteTransactionId = depositTransactionItem.TransactionId;
            item.WebSiteTransactionIdBigIntValue = ParseToLong(depositTransactionItem.TransactionId);
            item.DepositTransactionId = depositTransactionItem.Id;
            item.AdSoyad = depositTransactionItem.FullName;
            item.UserName = depositTransactionItem.UserName;
            item.UserId = depositTransactionItem.UserId;
            item.UserIdBigIntValue = ParseToLong(depositTransactionItem.UserId);

            if (availableHavaleBankItemDto.IsAutoSupplierBankAccount)
            {
                item.HesapSahibi = availableHavaleBankItemDto.HesapSahibi;
                item.HesapNo = availableHavaleBankItemDto.HesapNo;
                item.YontemTipi = $"Banka Havale ({availableHavaleBankItemDto.AutoSupplierBankName})";
            }
            else
            {
                item.HesapSahibi = havaleBankalariItem.HesapSahibi;
                item.HesapNo = havaleBankalariItem.HesapNo;
                item.YontemTipi = "Banka Havale";
            }

            _ = decimal.TryParse(depositTransactionItem.Amount, out decimal amount);
            item.Tutar = amount;

            item.TedarikciKomisyon = await GetTedarikciYatirimKomisyon(item.TedarikciId.Value, item.Tutar);
            item.WebSiteKomisyon = await GetWebSiteYatirimKomisyon(item.WebSiteId.Value, item.Tutar);
            item.Toplam = item.Tutar - item.TedarikciKomisyon;
            item.IslemTipi = (int)IslemTip.Yatirim;
            item.Durum = (int)Durum.Bekliyor;
            item.SilindiMi = false;
            item.KayitTarihi = DateTime.Now;

            await _yatirimCekimTalepleriRepository.InsertAsync(item);

            return webSiteItem.WebSiteUrl;
        }

        private async Task<decimal> GetTedarikciYatirimKomisyon(Guid tedarikciId, decimal tutar)
        {
            return await _tedarikcilerRepository.GetYatirimKomisyonu(tedarikciId, tutar);
        }

        private async Task<decimal> GetWebSiteYatirimKomisyon(Guid webSiteId, decimal tutar)
        {
            return await _webSiteRepository.GetYatirimKomisyonuAsync(webSiteId, tutar);
        }

        private Guid CreateDepositTransactionItem(ParaYolDepositCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto)
        {
            var depositCreateRequest = new DepositCreateRequest
            {
                UserName = request.UserName,
                UserId = request.UserId,
                TransactionId = request.TransactionId,
                FullName = request.FullName,
                Amount = request.Amount.ToString("0.##", CultureInfo.InvariantCulture),
                ClientId = request.ClientId,
                ClientSecret = request.ClientSecret,
                Hash = request.Hash
            };

            var depositTransactionId = _depositRepository.Insert(depositCreateRequest, webSitePaymentMethodDto);
            return depositTransactionId;
        }

        private bool CheckHash(ParaYolDepositCreateRequest request)
        {
            return _generalHelper.CheckHash(request.Hash, request.TransactionId, request.UserId, request.ClientId, request.ClientSecret);
        }
    }
}
