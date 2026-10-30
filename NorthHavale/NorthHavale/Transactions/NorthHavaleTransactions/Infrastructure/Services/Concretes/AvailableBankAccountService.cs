using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Infrastructure.Repositories.Interfaces;
using NorthHavaleTransactions.Infrastructure.Services.Interfaces;

namespace NorthHavaleTransactions.Infrastructure.Services.Concretes
{
    public class AvailableBankAccountService : IAvailableBankAccountService
    {
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly ITedarikcilerRepository _tedarikcilerRepository;
        private readonly ISettingsV2Repository _settingsV2Repository;
        private readonly IFluxFinansService _fluxFinansService;
        private readonly IViPayService _viPayService;
        private readonly IPullPayService _pullPayService;
        private readonly IParoPayService _paroPayService;
        private readonly IZenEntegreService _zenEntegreService;
        private readonly ITumBankService _tumBankService;
        private readonly IConfiguration _configuration;
        private readonly IFixHavaleService _fixHavaleService;

        public AvailableBankAccountService(
            IHavaleBankalariRepository havaleBankalariRepository,
            ITedarikcilerRepository tedarikcilerRepository,
            ISettingsV2Repository settingsV2Repository,
            IFluxFinansService fluxFinansService,
            IViPayService viPayService,
            IPullPayService pullPayService,
            IParoPayService paroPayService,
            IZenEntegreService zenEntegreService,
            ITumBankService tumBankService,
            IConfiguration configuration,
            IFixHavaleService fixHavaleService)
        {
            _havaleBankalariRepository = havaleBankalariRepository;
            _tedarikcilerRepository = tedarikcilerRepository;
            _settingsV2Repository = settingsV2Repository;
            _fluxFinansService = fluxFinansService;
            _viPayService = viPayService;
            _pullPayService = pullPayService;
            _paroPayService = paroPayService;
            _zenEntegreService = zenEntegreService;
            _tumBankService = tumBankService;
            _configuration = configuration;
            _fixHavaleService = fixHavaleService;
        }

        public async Task<AvailableHavaleBankItemDto?> GetValidBankAccountAsync(DepositTransaction depositTransaction, WebSiteleri webSiteItem, decimal amount, string bankaId)
        {
            var havaleBankalariItem = await GetAvailableHavaleBankItemDto(amount, webSiteItem.Id, bankaId);
            if (havaleBankalariItem == null)
            {
                return null;
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

            return havaleBankalariItem;
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

        private double GetTransasctionTimeLeftInSeconds(DateTime createdDate)
        {
            int validTransactionMinute = _configuration.GetValue<int>("ValidTransactionMinute");
            TimeSpan gecenSure2 = DateTime.Now - createdDate;
            return (validTransactionMinute * 60) - gecenSure2.TotalSeconds;
        }
    }
}
