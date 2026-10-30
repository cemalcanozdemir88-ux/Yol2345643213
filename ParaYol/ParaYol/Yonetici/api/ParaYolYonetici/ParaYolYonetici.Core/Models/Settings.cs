namespace ParaYolYonetici.Core.Models
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MaxFastTutar { get; set; }
        

        public Guid ViPayTedarikciId { get; set; }
        public Guid ViPayDefaultFastHavaleBankId { get; set; }
        public string ViPayDepositUrl { get; set; } = string.Empty;
        public string ViPayWithdrawUrl { get; set; } = string.Empty;
        public string ViPaySiteId { get; set; } = string.Empty;
        public string ViPayMethodId { get; set; } = string.Empty;
        public string ViPaySecretKey { get; set; } = string.Empty;
        public string ViPaySignKey { get; set; } = string.Empty;


        public Guid NhTedarikciId { get; set; }
        public Guid NhDefaultFastHavaleBankId { get; set; }
        public string NhDepositUrl { get; set; } = string.Empty;
        public string NhWithdrawUrl { get; set; } = string.Empty;

        public string NhSiteClientId { get; set; } = string.Empty;
        public string NhSiteSecretId { get; set; } = string.Empty;
    }
}
