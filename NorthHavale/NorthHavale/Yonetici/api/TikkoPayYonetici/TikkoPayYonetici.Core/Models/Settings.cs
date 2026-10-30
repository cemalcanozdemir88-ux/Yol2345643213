namespace TikkoPayYonetici.Core.Models
{
    public class Settings
    {
        public Guid Id { get; set; }
        public decimal MaxFastTutar { get; set; }
        public Guid AsPayTedarikciId { get; set; }
        public string AsPayAuthUrl { get; set; } 
        public string? AsPayToken { get; set; } 
        public DateTime? AsPayTokenExpiresAt { get; set; }
        public Guid AsPayDefaultFastHavaleBankaId { get; set; }
        public string AsPayDepositUrl { get; set; } 
        public string AsPayWithdrawUrl { get; set; }
        public string AsPayAppKey { get; set; } 
        public string AsPayAppSecret { get; set; } 
    }
}
