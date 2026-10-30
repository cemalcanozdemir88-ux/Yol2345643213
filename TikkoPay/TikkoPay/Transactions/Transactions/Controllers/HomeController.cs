using Microsoft.AspNetCore.Mvc;
using Transactions.Helpers;
using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;
using Transactions.Models;
using Transactions.Models.ViewModels;

namespace Transactions.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDepositRepository _depositRepository;
        private readonly IYatirimCekimTalepleriRepository _yatirimCekimTalepleriRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly IWebSiteRepository _webSiteRepository;
        private readonly ITedarikcilerRepository _tedarikcilerRepository;
        private readonly IFluxFinansService _fluxFinansService;
        private readonly IViPayService _viPayService;
        private readonly IPullPayService _pullPayService;
        private readonly IParoPayService _paroPayService;
        private readonly IZenEntegreService _zenEntegreService;
        private readonly ITumBankService _tumBankService;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IFixHavaleService _fixHavaleService;

        public HomeController(IConfiguration configuration,
                              IDepositRepository depositRepository,
                              IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                              IHavaleBankalariRepository havaleBankalariRepository,
                              IWebSiteRepository webSiteRepository,
                              ITedarikcilerRepository tedarikcilerRepository,
                              IFluxFinansService fluxFinansService,
                              IViPayService viPayService,
                              IPullPayService pullPayService,
                              IParoPayService paroPayService,
                              IZenEntegreService zenEntegreService,
                              ITumBankService tumBankService,
                              ISettingsV2Repository settingsV2Repository,
                              IFixHavaleService fixHavaleService)
        {
            _configuration = configuration;
            _depositRepository = depositRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _webSiteRepository = webSiteRepository;
            _tedarikcilerRepository = tedarikcilerRepository;
            _fluxFinansService = fluxFinansService;
            _viPayService = viPayService;
            _pullPayService = pullPayService;
            _paroPayService = paroPayService;
            _zenEntegreService = zenEntegreService;
            _tumBankService = tumBankService;
            _settingsV2Repository = settingsV2Repository;
            _fixHavaleService = fixHavaleService;
        }

        [FixDuplicateTransactionFilter]
        public async Task<IActionResult> Index(string transaction, string banka, bool isFast = false)
        {
            if (string.IsNullOrWhiteSpace(transaction))
                return NotFound();

            if (!Guid.TryParse(transaction, out var transactionGuidId))
                return NotFound();

            var depositTransaction = await _depositRepository.GetById(transactionGuidId);
            if (depositTransaction == null)
                return NotFound();

            if (!decimal.TryParse(depositTransaction.Amount, out var amount))
                return BadRequest("Invalid amount format.");

            var websiteItem = await _webSiteRepository.GetByIdAsync(depositTransaction.WebSiteId);
            if (amount < websiteItem.MinYatirimTutari)
                return BadRequest("Invalid amount");

            ViewBag.WebsiteTitle = websiteItem.Baslik;
            decimal fastAmount = 50000;

            var settings = await _settingsV2Repository.GetSettingsAsync();

            if (amount < fastAmount)
                return View();

            if (amount > settings.MaxFastTutar && string.IsNullOrWhiteSpace(banka))
                return Redirect($"/bank-list?transaction={transaction}");

            if (!isFast && string.IsNullOrWhiteSpace(banka))
                return Redirect($"/bank-list?transaction={transaction}");


            return View();
        }

        [Route("fast-options")]
        public async Task<IActionResult> FastOptions(string transaction)
        {
            var vieModel = new FastOptionsViewModel();

            _ = Guid.TryParse(transaction, out var transactionGuidId);
            var depositTransaction = await _depositRepository.GetById(transactionGuidId);
            if (depositTransaction == null)
            {
                vieModel.IsError = true;
                vieModel.ErrorMessage = "Transaction Bulunamadı!";

                return View(vieModel);
            }

            var websiteItem = await _webSiteRepository.GetByIdAsync(depositTransaction.WebSiteId);
            vieModel.WebsiteTitle = websiteItem.Baslik;
            vieModel.TransactionId = transactionGuidId;

            return View(vieModel);
        }

        [Route("bank-list")]
        public async Task<IActionResult> BankList(string transaction)
        {
            var vieModel = new BankListViewModel();

            _ = Guid.TryParse(transaction, out var transactionGuidId);
            var depositTransaction = await _depositRepository.GetById(transactionGuidId);
            if (depositTransaction == null)
            {
                vieModel.IsError = true;
                vieModel.ErrorMessage = "Transaction Bulunamadı!";

                return View(vieModel);
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();
            var maxFastTutar = settings.MaxFastTutar;

            var websiteItem = await _webSiteRepository.GetByIdAsync(depositTransaction.WebSiteId);
            vieModel.WebsiteTitle = websiteItem.Baslik;

            _ = decimal.TryParse(depositTransaction.Amount, out var amout);

            if(amout <= maxFastTutar)
            {
                vieModel.ShowFastBlock = true;
            }
            else 
            {
                vieModel.ShowFastBlock = false;
            }

            vieModel.IsError = false;
            vieModel.ErrorMessage = string.Empty;
            vieModel.TransactionId = transactionGuidId;
            vieModel.Bankalar = await _havaleBankalariRepository.GetBankListForAmountOverFast(amout);

            return View(vieModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetValidBankAccount([FromBody] GetValidBankAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TransactionId))
            {
                return Json(new { Message = "Transaction Bulunamadı!", status = false });
            }

            _ = Guid.TryParse(request.TransactionId, out var transactionGuidId);
            var depositTransaction = await _depositRepository.GetById(transactionGuidId);
            if (depositTransaction == null)
            {
                return Json(new { Message = "Transaction Bulunamadı!", status = false });
            }

            if (IsTransactionExpired(depositTransaction.KayitTarihi))
            {
                return Json(new { Message = "İşlem zaman aşımına uğradı!", status = false });
            }

            var webSiteItem = await _webSiteRepository.GetByIdAsync(depositTransaction.WebSiteId);
            if (webSiteItem == null)
            {
                return Json(new { Message = "WebSite Bulunamadı!", status = false });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetCountByWebSiteTransactionId(depositTransaction.TransactionId);
            if (yatirimCekimTalepleriItem > 0)
            {
                return Json(new { Message = "Geçersiz transaction!", status = false });
            }

            _ = decimal.TryParse(depositTransaction.Amount, out decimal amount);

            var pendingYatirimTalepItem = await _yatirimCekimTalepleriRepository.GetPendingYatirimTalepleri(depositTransaction.TransactionId);
            if (pendingYatirimTalepItem != null)
            {
                var viewModelPending = new HavaleBankaViewModel
                {
                    HesapSahibi = pendingYatirimTalepItem.HesapSahibi,
                    HesapNo = pendingYatirimTalepItem.HesapNo,
                    Amount = amount.ToString("N2"),
                    TimeLeft = GetTransasctionTimeLeftInSeconds(depositTransaction.KayitTarihi),
                    WebSiteUrl = webSiteItem.WebSiteUrl,
                    Fullname = depositTransaction.FullName,
                    Username = depositTransaction.UserName,
                };

                return Json(new { Message = "", status = true, Model = viewModelPending });
            }

            var havaleBankalariItem = await GetAvailableHavaleBankItemDto(amount, webSiteItem.Id, request.BankaId);
            if (havaleBankalariItem == null)
            {
                return Json(new { Message = "Hesap Bulunamadı!", status = false });
            }

            var settings = await _settingsV2Repository.GetSettingsAsync();

            if (havaleBankalariItem.TedarikciId.Equals(settings.FluxFinansTedarikciId))
            {
                havaleBankalariItem = await GetFluxFinansBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.ViPayTedarikciId))
            {
                havaleBankalariItem = await GetViPayBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.PullPayTedarikciId))
            {
                havaleBankalariItem = await GetPullPayBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.ParoPayTedarikciId))
            {
                havaleBankalariItem = await GetParoPayBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.ZenEntegreTedarikciId))
            {
                havaleBankalariItem = await GetZenEntegreBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.TumBankTedarikciId))
            {
                havaleBankalariItem = await GetTumBankBankAccount(depositTransaction, settings, amount);
            }
            else if (havaleBankalariItem.TedarikciId.Equals(settings.FixHavaleTedarikciId))
            {
                havaleBankalariItem = await GetFixHavaleBankAccount(depositTransaction, settings, amount);
            }

            havaleBankalariItem.Amount = amount.ToString("N2");
            havaleBankalariItem.TimeLeft = GetTransasctionTimeLeftInSeconds(depositTransaction.KayitTarihi);
            havaleBankalariItem.WebSiteUrl = webSiteItem.WebSiteUrl;
            havaleBankalariItem.UserId = depositTransaction.UserId;
            havaleBankalariItem.Username = depositTransaction.UserName;
            havaleBankalariItem.Fullname = depositTransaction.FullName;

            await SendTransaction(depositTransaction, havaleBankalariItem);

            var viewModel = new HavaleBankaViewModel
            {
                HesapSahibi = havaleBankalariItem.HesapSahibi,
                HesapNo = havaleBankalariItem.HesapNo,
                Amount = havaleBankalariItem.Amount,
                TimeLeft = havaleBankalariItem.TimeLeft,
                WebSiteUrl = havaleBankalariItem.WebSiteUrl,
                Fullname = havaleBankalariItem.Fullname,
                Username = havaleBankalariItem.Username,
            };

            return Json(new { Message = "", status = true, Model = viewModel });
        }

        private async Task<AvailableHavaleBankItemDto> GetFluxFinansBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _fluxFinansService.GetBankAccountAsync(depositTransaction, amount);

            return new AvailableHavaleBankItemDto
            {
                Id = settings.FluxFinansDefaultFastHavaleBankId,
                HesapNo = bankAccount.Data.Account.Iban,
                HesapSahibi = bankAccount.Data.Account.AccountHolderName,
                AutoSupplierBankName = bankAccount.Data.Account.BankName,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetViPayBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _viPayService.GetBankAccountAsync(depositTransaction, amount);

            return new AvailableHavaleBankItemDto
            {
                Id = settings.ViPayDefaultFastHavaleBankId,
                HesapNo = bankAccount.data.iban,
                HesapSahibi = bankAccount.data.account_holder,
                AutoSupplierBankName = bankAccount.data.bank,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetFixHavaleBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _fixHavaleService.GetBankAccountAsync(depositTransaction, amount);

            return new AvailableHavaleBankItemDto
            {
                Id = settings.FixHavaleDefaultFastHavaleBankId,
                HesapNo = bankAccount.BankAccount.Iban,
                HesapSahibi = bankAccount.BankAccount.AccountName,
                AutoSupplierBankName = bankAccount.BankAccount.BankName,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetPullPayBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _pullPayService.GetBankAccountAsync(depositTransaction, amount);

            return new AvailableHavaleBankItemDto
            {
                Id = settings.PullPayFastHavaleBankId,
                HesapNo = bankAccount.AccountIban,
                HesapSahibi = bankAccount.AccountName,
                AutoSupplierBankName = string.Empty,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetParoPayBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _paroPayService.GetBankAccountAsync(depositTransaction, amount);
            return new AvailableHavaleBankItemDto
            {
                Id = settings.ParoPayFastHavaleBankaId,
                HesapNo = bankAccount.Iban,
                HesapSahibi = bankAccount.IbanOwner,
                AutoSupplierBankName = bankAccount.BankName,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetZenEntegreBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _zenEntegreService.GetBankAccountAsync(depositTransaction, amount);
            return new AvailableHavaleBankItemDto
            {
                Id = settings.ZenEntegreDefaultFastHavaleBankId,
                HesapNo = bankAccount.Transaction.Account.AccountIban,
                HesapSahibi = bankAccount.Transaction.Account.AccountHolder,
                AutoSupplierBankName = bankAccount.Transaction.Account.AccountBankName,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetTumBankBankAccount(DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            var bankAccount = await _tumBankService.GetBankAccountAsync(depositTransaction, amount);
            return new AvailableHavaleBankItemDto
            {
                Id = settings.TumBankDefaultFastHavaleBankId,
                HesapNo = bankAccount.Data.Iban,
                HesapSahibi = bankAccount.Data.AccountHolder,
                AutoSupplierBankName = bankAccount.Data.BankName,
                IsAutoSupplierBankAccount = true,
            };
        }

        private async Task<AvailableHavaleBankItemDto> GetAvailableHavaleBankItemDto(decimal amount, Guid webSiteId, string bankaId)
        {
            var havaleBankalariItem = await _havaleBankalariRepository.GetAvailableHavaleBankItemDto(amount, webSiteId, bankaId);
            if (havaleBankalariItem == null)
            {
                await _tedarikcilerRepository.ResetHesapNoGosterimSayisi();
                return await _havaleBankalariRepository.GetAvailableHavaleBankItemDto(amount, webSiteId, bankaId);
            }

            return havaleBankalariItem;
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

        private int GetValidTransactionMinute()
        {
            return _configuration.GetValue<int>("ValidTransactionMinute");
        }

        private bool IsTransactionExpired(DateTime createdDate)
        {
            int validTransactionMinute = GetValidTransactionMinute();
            TimeSpan gecenSure = DateTime.Now - createdDate;

            return gecenSure.TotalMinutes > validTransactionMinute;
        }

        private double GetTransasctionTimeLeftInSeconds(DateTime createdDate)
        {
            int validTransactionMinute = GetValidTransactionMinute();
            TimeSpan gecenSure2 = DateTime.Now - createdDate;
            return (validTransactionMinute * 60) - gecenSure2.TotalSeconds;
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
    }
}
