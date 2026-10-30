using PaparaTedarikciHizmet.Core.Enums;
using System.Security.Cryptography;

namespace PaparaTedarikciHizmet.Core.Models
{
    public class Kullanicilar : BaseModel
    {
        public Kullanicilar()
        {
            
        }

        public Guid? TedarikciId { get; set; }
        public Guid? WebSiteId { get; set; }
        public string AdSoyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public KullaniciTip KullaniciTipi { get; set; }
        public bool Durum { get; set; }
        public bool IsOtpEnabled { get; set; }

        public Tedarikciler Tedarikciler { get; set; }
        public WebSiteleri WebSiteleri { get; set; }
        public ICollection<YatirimCekimTalepleri> YatirimTalepleri { get; set; }
        public ICollection<KasaHaraketleri> KasaHaraketleriKaydeden { get; set; }
        public ICollection<KasaHaraketleri> KasaHaraketleriGuncelleyen { get; set; }
        public ICollection<WebSiteKasaHaraketleri> WebSiteKasaHaraketleriKaydeden { get; set; }
        public ICollection<WebSiteKasaHaraketleri> WebSiteKasaHaraketleriGuncelleyen { get; set; }
        public ICollection<KullaniciYetkileri> KullaniciYetkileri { get; set; }

        public void SetPassword(string password)
        {
            using var hmac = new HMACSHA256();
            PasswordSalt = hmac.Key;
            PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPassword(string password)
        {
            using var hmac = new HMACSHA256(PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != PasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
