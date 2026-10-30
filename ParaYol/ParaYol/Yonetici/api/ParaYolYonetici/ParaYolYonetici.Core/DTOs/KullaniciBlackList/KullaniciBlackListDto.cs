namespace ParaYolYonetici.Core.DTOs.KullaniciBlackList
{
    public class KullaniciBlackListDto
    {
        public Guid Id { get; set; }

        public Guid WebSiteId { get; set; }

        public string WebSiteUrl { get; set; } = string.Empty;

        public long UserId { get; set; }
    }
}
