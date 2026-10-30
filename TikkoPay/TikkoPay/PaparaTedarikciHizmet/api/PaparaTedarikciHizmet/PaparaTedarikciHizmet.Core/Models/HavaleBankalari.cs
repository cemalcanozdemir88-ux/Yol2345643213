namespace PaparaTedarikciHizmet.Core.Models
{
    public class HavaleBankalari : BaseModel
    {
        public Guid TedarikciId { get; set; }
        public Guid BankaId { get; set; }

        public string HesapSahibi { get; set; }
        public string HesapNo { get; set; }
        public decimal HesapMinTutar { get; set; }
        public decimal HesapMaxTutar { get; set; }
        public int MaxYatirimAdedi { get; set; }
        public decimal MaxYatirimToplami { get; set; }
        public int OnaylananYatirimAdedi { get; set; }
        public int OnaylananYatirimAdediTemp { get; set; }
        public decimal OnaylananYatirimToplami { get; set; }
        public bool Durum { get; set; }

        public Tedarikciler Tedarikciler { get; set; }
        public Bankalar Bankalar { get; set; }
    }
}
