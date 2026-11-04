namespace TikkoPayYonetici.Core.Models
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MaxFastTutar { get; set; }
        
        public Guid FluxFinansTedarikciId { get; set; }
        public Guid FluxFinansDefaultFastHavaleBankId { get; set; }
        public string FluxFinansAuthUrl { get; set; } = string.Empty;
        public string FluxFinansDepositUrl { get; set; } = string.Empty;
        public string FluxFinansWithdrawUrl { get; set; } = string.Empty;
        public string FluxFinansSid { get; set; } = string.Empty;
        public string FluxFinansApiKey { get; set; } = string.Empty;
        public string FluxFinansSecretKey { get; set; } = string.Empty;

        public Guid ViPayTedarikciId { get; set; }
        public Guid ViPayTedarikciId2 { get; set; }
        public Guid ViPayDefaultFastHavaleBankId { get; set; }
        public string ViPayDepositUrl { get; set; } = string.Empty;
        public string ViPayWithdrawUrl { get; set; } = string.Empty;
        public string ViPaySiteId { get; set; } = string.Empty;
        public string ViPayMethodId { get; set; } = string.Empty;
        public string ViPaySecretKey { get; set; } = string.Empty;
        public string ViPaySignKey { get; set; } = string.Empty;

        public Guid PullPayTedarikciId { get; set; }
        public Guid PullPayFastHavaleBankId { get; set; }
        public string PullPayEmail { get; set; } = string.Empty;
        public string PullPayPassword { get; set; } = string.Empty;
        public int PullPayWebsiteId { get; set; }
        public string PullPayDepositUrl { get; set; } = string.Empty;
        public string PullPayWithdrawUrl { get; set; } = string.Empty;
        public string PullPayCallbackUrl { get; set; } = string.Empty;

        public Guid ParoPayTedarikciId { get; set; }
        public Guid ParoPayFastHavaleBankaId { get; set; }
        public string ParoPayDepositUrl { get; set; } = string.Empty;
        public string ParoPayWithdrawUrl { get; set; } = string.Empty;
        public string ParoPayApiKey { get; set; } = string.Empty;
        public string ParoPayApiSecret { get; set; } = string.Empty;

        public Guid ZenEntegreTedarikciId { get; set; }
        public Guid ZenEntegreDefaultFastHavaleBankId { get; set; }
        public string ZenEntegreMid { get; set; } = string.Empty;
        public string ZenEntegreSecret { get; set; } = string.Empty;
        public string ZenEntegreDepositUrl { get; set; } = string.Empty;
        public string ZenEntegreWithdrawUrl { get; set; } = string.Empty;

        public Guid TumBankTedarikciId { get; set; }
        public Guid TumBankDefaultFastHavaleBankId { get; set; }
        public string TumBankApiKey { get; set; } = string.Empty;
        public string TumBankApiSecret { get; set; } = string.Empty;
        public string TumBankDepositUrl { get; set; } = string.Empty;
        public string TumBankWithdrawUrl { get; set; } = string.Empty;

        public Guid FixHavaleTedarikciId { get; set; }
        public Guid FixHavaleDefaultFastHavaleBankId { get; set; }
        public string FixHavaleDepositUrl { get; set; } = string.Empty;
        public string FixHavaleApiKey { get; set; } = string.Empty;
        public string FixHavaleApiSecret { get; set; } = string.Empty;
        public string FixHavaleWithdrawUrl { get; set; } = string.Empty;
    }
}
