namespace ParaYolYonetici.Core.Models
{
    public class WebSitePaymentMethods : BaseModel
    {
        public Guid WebSiteleriId { get; set; }
        public int PaymentMethodId { get; set; }
        public string ApiClientId { get; set; } = string.Empty;
        public string ApiClientSecret { get; set; } = string.Empty;
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
        public int Type { get; set; }

        public PaymentMethods? PaymentMethods { get; set; }
    }
}
