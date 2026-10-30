namespace TikkoPayYonetici.Core.Models
{
    public class KullaniciYetkileri : BaseModel
    {
        public KullaniciYetkileri()
        {
            
        }

        public Guid KullaniciId { get; set; }
        public Guid YetkiId { get; set; }

        public Kullanicilar Kullanicilar { get; set; }
        public Yetkiler Yetkiler { get; set; }
    }
}
