namespace TikkoPayYonetici.Core.DTOs.PaymentMethods
{
    public class PaymentMethodsDto
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public int PaymentId { get; set; }
        public string DepositCallbackUrl { get; set; } = string.Empty;
        public string WithdrawCallbackUrl { get; set; } = string.Empty;
    }
}
