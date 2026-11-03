using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Infrastructure.Repositories.Interfaces;
using Transactions.Infrastructure.Services.Interfaces;

namespace Transactions.Controllers
{
    /// <summary>
    /// Sadece Yönetici API'sinin çağırdığı dahili endpoint.
    /// İptal edilen bir yatırım yöneticide "Yeniden Gönder" ile API'li bir tedarikçiye yönlendirildiğinde,
    /// normal yatırım akışındaki aynı servisle tedarikçiye istek atar ve tedarikçinin verdiği hesabı döner.
    /// Veritabanına kayıt yazmaz; yeni yatırım kaydını Yönetici API'si açar.
    /// </summary>
    [Route("api/internal/deposit")]
    [ApiController]
    public class InternalDepositController : ControllerBase
    {
        private const string ApiKeyHeader = "X-Internal-Api-Key";

        private readonly IConfiguration _configuration;
        private readonly IDepositRepository _depositRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IFluxFinansService _fluxFinansService;
        private readonly IViPayService _viPayService;
        private readonly IPullPayService _pullPayService;
        private readonly IParoPayService _paroPayService;
        private readonly IZenEntegreService _zenEntegreService;
        private readonly ITumBankService _tumBankService;
        private readonly IFixHavaleService _fixHavaleService;

        public InternalDepositController(IConfiguration configuration,
                                         IDepositRepository depositRepository,
                                         IHavaleBankalariRepository havaleBankalariRepository,
                                         ISettingsV2Repository settingsV2Repository,
                                         IFluxFinansService fluxFinansService,
                                         IViPayService viPayService,
                                         IPullPayService pullPayService,
                                         IParoPayService paroPayService,
                                         IZenEntegreService zenEntegreService,
                                         ITumBankService tumBankService,
                                         IFixHavaleService fixHavaleService)
        {
            _configuration = configuration;
            _depositRepository = depositRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _settingsV2Repository = settingsV2Repository;
            _fluxFinansService = fluxFinansService;
            _viPayService = viPayService;
            _pullPayService = pullPayService;
            _paroPayService = paroPayService;
            _zenEntegreService = zenEntegreService;
            _tumBankService = tumBankService;
            _fixHavaleService = fixHavaleService;
        }

        [HttpPost("send-to-supplier")]
        public async Task<IActionResult> SendToSupplier(SendDepositToSupplierRequest request)
        {
            if (!IsAuthorized())
                return Unauthorized();

            var depositTransaction = await _depositRepository.GetByTransactionIdAsync(request.WebSiteTransactionId);
            if (depositTransaction == null)
                return BadRequest(new SendDepositToSupplierResponse { Success = false, Message = "Deposit transaction bulunamadı." });

            var settings = await _settingsV2Repository.GetSettingsAsync();

            AvailableHavaleBankItemDto bankAccount;
            try
            {
                bankAccount = await GetSupplierBankAccount(request.TedarikciId, depositTransaction, settings, request.Amount);
            }
            catch (Exception ex)
            {
                // Tedarikçinin ret sebebi yöneticiye gösterilsin (ör. aynı transaction daha önce kullanılmış)
                return BadRequest(new SendDepositToSupplierResponse { Success = false, Message = $"Tedarikçi isteği kabul etmedi: {ex.Message}" });
            }

            if (bankAccount == null)
                return BadRequest(new SendDepositToSupplierResponse { Success = false, Message = "Seçilen tedarikçi API ile çalışan bir tedarikçi değil." });

            if (string.IsNullOrWhiteSpace(bankAccount.HesapNo))
                return BadRequest(new SendDepositToSupplierResponse { Success = false, Message = "Tedarikçi hesap bilgisi döndürmedi." });

            var havaleBankalariItem = await _havaleBankalariRepository.GetById(bankAccount.Id);

            return Ok(new SendDepositToSupplierResponse
            {
                Success = true,
                HavaleBankaId = bankAccount.Id,
                BankaId = havaleBankalariItem?.BankaId,
                HesapNo = bankAccount.HesapNo,
                HesapSahibi = bankAccount.HesapSahibi,
                YontemTipi = $"Banka Havale ({bankAccount.AutoSupplierBankName})"
            });
        }

        // HomeController'daki tedarikçi eşleştirmesinin aynısı
        private async Task<AvailableHavaleBankItemDto> GetSupplierBankAccount(Guid tedarikciId, DepositTransaction depositTransaction, Settings settings, decimal amount)
        {
            // Tedarikçi hata döndüğünde hesap bilgisi boş gelir; bu durumda tedarikçinin kendi mesajı gösterilir.
            if (tedarikciId.Equals(settings.FluxFinansTedarikciId))
            {
                var r = await _fluxFinansService.GetBankAccountAsync(depositTransaction, amount);
                var a = r?.Data?.Account;
                return Build(settings.FluxFinansDefaultFastHavaleBankId, a?.Iban, a?.AccountHolderName, a?.BankName, r?.Message);
            }
            if (tedarikciId.Equals(settings.ViPayTedarikciId))
            {
                var r = await _viPayService.GetBankAccountAsync(depositTransaction, amount);
                return Build(settings.ViPayDefaultFastHavaleBankId, r?.data?.iban, r?.data?.account_holder, r?.data?.bank, r?.message);
            }
            if (tedarikciId.Equals(settings.PullPayTedarikciId))
            {
                var r = await _pullPayService.GetBankAccountAsync(depositTransaction, amount);
                return Build(settings.PullPayFastHavaleBankId, r?.AccountIban, r?.AccountName, string.Empty, r?.Message);
            }
            if (tedarikciId.Equals(settings.ParoPayTedarikciId))
            {
                var r = await _paroPayService.GetBankAccountAsync(depositTransaction, amount);
                return Build(settings.ParoPayFastHavaleBankaId, r?.Iban, r?.IbanOwner, r?.BankName, r?.Message);
            }
            if (tedarikciId.Equals(settings.ZenEntegreTedarikciId))
            {
                var r = await _zenEntegreService.GetBankAccountAsync(depositTransaction, amount);
                var a = r?.Transaction?.Account;
                return Build(settings.ZenEntegreDefaultFastHavaleBankId, a?.AccountIban, a?.AccountHolder, a?.AccountBankName, r?.Message);
            }
            if (tedarikciId.Equals(settings.TumBankTedarikciId))
            {
                var r = await _tumBankService.GetBankAccountAsync(depositTransaction, amount);
                return Build(settings.TumBankDefaultFastHavaleBankId, r?.Data?.Iban, r?.Data?.AccountHolder, r?.Data?.BankName, r?.Message);
            }
            if (tedarikciId.Equals(settings.FixHavaleTedarikciId))
            {
                var r = await _fixHavaleService.GetBankAccountAsync(depositTransaction, amount);
                var message = r?.Message;
                if (!string.IsNullOrWhiteSpace(r?.DetectedIp))
                    message = $"{message} (tedarikçinin gördüğü IP: {r.DetectedIp})";
                return Build(settings.FixHavaleDefaultFastHavaleBankId, r?.BankAccount?.Iban, r?.BankAccount?.AccountName, r?.BankAccount?.BankName, message);
            }

            return null;
        }

        private static AvailableHavaleBankItemDto Build(Guid havaleBankaId, string iban, string hesapSahibi, string bankaAdi, string tedarikciMesaji)
        {
            if (string.IsNullOrWhiteSpace(iban))
            {
                throw new Exception(string.IsNullOrWhiteSpace(tedarikciMesaji)
                    ? "Tedarikçi hesap bilgisi döndürmedi."
                    : tedarikciMesaji);
            }

            return new AvailableHavaleBankItemDto
            {
                Id = havaleBankaId,
                HesapNo = iban,
                HesapSahibi = hesapSahibi,
                AutoSupplierBankName = bankaAdi,
                IsAutoSupplierBankAccount = true
            };
        }

        private bool IsAuthorized()
        {
            var expected = _configuration.GetValue<string>("InternalApiKey");
            if (string.IsNullOrWhiteSpace(expected))
                return false;

            if (!Request.Headers.TryGetValue(ApiKeyHeader, out var provided))
                return false;

            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(provided.ToString()));
        }
    }

    public class SendDepositToSupplierRequest
    {
        public string WebSiteTransactionId { get; set; } = string.Empty;
        public Guid TedarikciId { get; set; }
        public decimal Amount { get; set; }
    }

    public class SendDepositToSupplierResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? HavaleBankaId { get; set; }
        public Guid? BankaId { get; set; }
        public string HesapNo { get; set; } = string.Empty;
        public string HesapSahibi { get; set; } = string.Empty;
        public string YontemTipi { get; set; } = string.Empty;
    }
}
