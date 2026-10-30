namespace TikkoPayYonetici.Core.DTOs.WebSitePaymentMethods
{
    public class WebSitePaymentMethodsDto
    {
        public Guid Id { get; set; }
        public Guid WebSiteleriId { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentMethodBaslik { get; set; } = string.Empty;
    }
}
