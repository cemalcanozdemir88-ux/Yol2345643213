namespace Transactions.Infrastructure.Dtos
{
    public class WebSitePaymentMethodsDto
    {
        public Guid Id { get; set; }
        public Guid WebSiteleriId { get; set; }
        public double YaririmKomisyonOrani { get; set; }
        public double CekimKomisyonOrani { get; set; }
        public decimal BakiyeLimitor { get; set; }
        public decimal MinYatirimTutari { get; set; }
        public decimal MinCekimTutari { get; set; }
    }
}
