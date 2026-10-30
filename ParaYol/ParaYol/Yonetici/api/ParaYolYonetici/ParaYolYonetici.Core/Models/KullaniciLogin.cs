namespace ParaYolYonetici.Core.Models
{
    public class KullaniciLogin
    {
        public Guid Id { get; set; }
        public string Eposta { get; set; }
        public string ErrorMessage { get; set; }
        public string DetailMessage { get; set; }
        public bool IsSuccess { get; set; }
        public string IpAddress { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
