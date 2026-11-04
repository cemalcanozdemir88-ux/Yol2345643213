namespace Transactions.Infrastructure.Entities
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MaxFastTutar { get; set; }
        
        public string IbanYasakliListesi { get; set; } = string.Empty;

        public Guid FluxFinansTedarikciId { get; set; }
        public Guid FluxFinansDefaultFastHavaleBankId { get; set; }
        public string FluxFinansAuthUrl { get; set; }
        public string FluxFinansDepositUrl { get; set; }
        public string FluxFinansWithdrawUrl { get; set; }
        public string FluxFinansSid { get; set; }
        public string FluxFinansApiKey { get; set; }
        public string FluxFinansSecretKey { get; set; }

        public Guid ViPayTedarikciId { get; set; }
        public Guid ViPayTedarikciId2 { get; set; }
        public Guid ViPayDefaultFastHavaleBankId { get; set; }
        public Guid ViPayDefaultFastHavaleBankId2 { get; set; }
        public string ViPayDepositUrl { get; set; }
        public string ViPayWithdrawUrl { get; set; }
        public string ViPaySiteId { get; set; }
        public string ViPayMethodId { get; set; }
        public string ViPaySecretKey { get; set; }

        public Guid PullPayTedarikciId { get; set; }
        public Guid PullPayFastHavaleBankId { get; set; }
        public string PullPayEmail { get; set; }
        public string PullPayPassword { get; set; }
        public int PullPayWebsiteId { get; set; }
        public string PullPayDepositUrl { get; set; }
        public string PullPayWithdrawUrl { get; set; }
        public string PullPayCallbackUrl { get; set; }

        public Guid ParoPayTedarikciId { get; set; }
        public Guid ParoPayFastHavaleBankaId { get; set; }
        public string ParoPayDepositUrl { get; set; }
        public string ParoPayWithdrawUrl { get; set; }
        public string ParoPayApiKey { get; set; }
        public string ParoPayApiSecret { get; set; }

        public Guid ZenEntegreTedarikciId { get; set; }
        public Guid ZenEntegreDefaultFastHavaleBankId { get; set; }
        public string ZenEntegreMid { get; set; }
        public string ZenEntegreSecret { get; set; }
        public string ZenEntegreDepositUrl { get; set; }
        public string ZenEntegreWithdrawUrl { get; set; }

        public Guid TumBankTedarikciId { get; set; }
        public Guid TumBankDefaultFastHavaleBankId { get; set; }
        public string TumBankApiKey { get; set; }
        public string TumBankApiSecret { get; set; }
        public string TumBankDepositUrl { get; set; }
        public string TumBankWithdrawUrl { get; set; }

        public Guid FixHavaleTedarikciId { get; set; }
        public Guid FixHavaleDefaultFastHavaleBankId { get; set; }
        public string FixHavaleDepositUrl { get; set; }
        public string FixHavaleApiKey { get; set; }
        public string FixHavaleApiSecret { get; set; }
        public string FixHavaleWithdrawUrl { get; set; }
    }
}
