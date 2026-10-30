namespace ParaYolCallbacks.Infrastructure.Entities
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MaxFastTutar { get; set; }

        public string IbanYasakliListesi { get; set; } = string.Empty;

        public Guid ViPayTedarikciId { get; set; }
        public Guid ViPayDefaultFastHavaleBankId { get; set; }
        public string ViPayDepositUrl { get; set; }
        public string ViPayWithdrawUrl { get; set; }
        public string ViPaySiteId { get; set; }
        public string ViPayMethodId { get; set; }
        public string ViPaySecretKey { get; set; }
        public string ViPaySignKey { get; set; }

        public Guid NhTedarikciId { get; set; }
        public Guid NhDefaultFastHavaleBankId { get; set; }
        public string NhDepositUrl { get; set; }
        public string NhWithdrawUrl { get; set; }
        public string NhSiteId { get; set; }
        public string NhMethodId { get; set; }
        public string NhSecretKey { get; set; }
    }
}
