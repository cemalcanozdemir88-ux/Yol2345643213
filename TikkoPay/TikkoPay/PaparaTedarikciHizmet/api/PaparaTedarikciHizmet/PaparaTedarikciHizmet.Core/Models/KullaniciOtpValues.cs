namespace PaparaTedarikciHizmet.Core.Models
{
    public class KullaniciOtpValues
    {
        public Guid Id { get; set; }
        public Guid KullaniciId { get; set; }
        public bool SilindiMi { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
