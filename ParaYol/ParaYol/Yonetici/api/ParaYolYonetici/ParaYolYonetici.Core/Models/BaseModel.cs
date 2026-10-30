namespace ParaYolYonetici.Core.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public bool SilindiMi { get; set; }

        public DateTime KayitTarihi { get; set; }
        public Guid? KaydedenKullaniciId { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public Guid? GuncelleyenKullaniciId { get; set; }
    }
}
