using Microsoft.AspNetCore.Mvc;
using Transactions.Helpers;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Helpers;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Models;

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

        public HomeController(IConfiguration configuration,
                              IDepositRepository depositRepository,
                              IYatirimCekimTalepleriRepository yatirimCekimTalepleriRepository,
                              IHavaleBankalariRepository havaleBankalariRepository,
                              IWebSiteRepository webSiteRepository,
                              ITedarikcilerRepository tedarikcilerRepository)
        {
            _configuration = configuration;
            _depositRepository = depositRepository;
            _yatirimCekimTalepleriRepository = yatirimCekimTalepleriRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _webSiteRepository = webSiteRepository;
            _tedarikcilerRepository = tedarikcilerRepository;
        }

        [FixDuplicateTransactionFilter]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetValidBankAccount([FromBody] string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return Json(new { Message = "Transaction Bulunamadı!", status = false });
            }

            _ = Guid.TryParse(transactionId, out var transactionGuidId);
            var depositTransaction = await _depositRepository.GetById(transactionGuidId);
            if (depositTransaction == null)
            {
                return Json(new { Message = "Transaction Bulunamadı!", status = false });
            }

            if (IsTransactionExpired(depositTransaction.KayitTarihi))
            {
                return Json(new { Message = "İşlem zaman aşımına uğradı!", status = false });
            }

            var yatirimCekimTalepleriItem = await _yatirimCekimTalepleriRepository.GetCountByWebSiteTransactionId(depositTransaction.TransactionId);
            if (yatirimCekimTalepleriItem > 0)
            {
                return Json(new { Message = "Geçersiz transaction!", status = false });
            }

            var pendingTransactionsCount = await _depositRepository.GetPendingTransactionsCountByUserId(depositTransaction.WebSiteId, depositTransaction.UserId);
            if (pendingTransactionsCount > 0)
            {
                return Json(new { Message = "Beklemede olan işleminiz mevcut. Yeni talep oluşturamazsınız!", status = false });
            }

            _ = decimal.TryParse(depositTransaction.Amount, out decimal amount);

            var havaleBankalariItem = await _havaleBankalariRepository.GetAvailableHavaleBankItemDto(amount);
            if (havaleBankalariItem == null)
            {
                return Json(new { Message = "Hesap Bulunamadı!", status = false });
            }

            havaleBankalariItem.Amount = amount.ToString("N2");
            havaleBankalariItem.TimeLeft = GetTransasctionTimeLeftInSeconds(depositTransaction.KayitTarihi);

            return Json(new { Message = "", status = true, Model = havaleBankalariItem });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTransaction([FromBody] SendTransactionRequestModel requestModel)
        {
            _ = Guid.TryParse(requestModel.HavaleBankId, out var havaleBankGuidId);

            var havaleBankalariItem = await _havaleBankalariRepository.GetById(havaleBankGuidId);
            if (havaleBankalariItem == null)
            {
                return Json(new { Message = "Hesap Bulunamadı!", status = false });
            }

            _ = Guid.TryParse(requestModel.TransactionId, out var transactionGuidId);

            var depositTransactionItem = await _depositRepository.GetById(transactionGuidId);
            if (depositTransactionItem == null)
            {
                return Json(new { Message = "Transaction Bulunamadı!", status = false });
            }

            TimeSpan gecenSure = DateTime.Now - depositTransactionItem.KayitTarihi;
            if (gecenSure.TotalMinutes > GetValidTransactionMinute())
            {
                return Json(new { Message = "İşlem zaman aşımına uğradı!", status = false });
            }

            var webSiteItem = await _webSiteRepository.GetById(depositTransactionItem.WebSiteId);
            if (webSiteItem == null)
            {
                return Json(new { Message = "WebSite Bulunamadı!", status = false });
            }

            var item = new YatirimCekimTalepleri
            {
                Id = Guid.NewGuid(),
                TalepNo = DateTime.Now.ToString("yyyyMMdd"),
                TedarikciId = havaleBankalariItem.TedarikciId,
                HavaleBankaId = havaleBankalariItem.Id,
                BankaId = havaleBankalariItem.BankaId,
                WebSiteId = depositTransactionItem.WebSiteId,
                WebSiteTransactionId = depositTransactionItem.TransactionId,
                AdSoyad = depositTransactionItem.FullName,
                UserName = depositTransactionItem.UserName,
                UserId = depositTransactionItem.UserId,
                YontemTipi = "Papara Havale",
                HesapSahibi = havaleBankalariItem.HesapSahibi,
                HesapNo = havaleBankalariItem.HesapNo
            };

            _ = decimal.TryParse(depositTransactionItem.Amount, out decimal amount);
            item.Tutar = amount;

            item.TedarikciKomisyon = await GetTedarikciYatirimKomisyon(item.TedarikciId.Value, item.Tutar);
            item.WebSiteKomisyon = await GetWebSiteYatirimKomisyon(item.WebSiteId.Value, item.Tutar);
            item.Toplam = item.Tutar - item.TedarikciKomisyon;
            item.IslemTipi = (int)IslemTip.Yatirim;
            item.Durum = (int)Durum.Bekliyor;
            item.SilindiMi = false;
            item.KayitTarihi = DateTime.Now;

            _yatirimCekimTalepleriRepository.Insert(item);

            return Json(new { Message = "", status = true, Model = webSiteItem.WebSiteUrl });
        }

        private async Task<decimal> GetTedarikciYatirimKomisyon(Guid tedarikciId, decimal tutar)
        {
            return await _tedarikcilerRepository.GetYatirimKomisyonu(tedarikciId, tutar);
        }

        private async Task<decimal> GetWebSiteYatirimKomisyon(Guid webSiteId, decimal tutar)
        {
            return await _webSiteRepository.GetYatirimKomisyonu(webSiteId, tutar);
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
    }
}
