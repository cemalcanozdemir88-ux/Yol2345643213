namespace TikkoPayCallbacks.Models
{
    /// <summary>
    /// BluePara'nın sorgulama servisinden (GET) okunan, güvenilir işlem bilgisi.
    /// Callback gövdesindeki değerler yerine bu bilgiler kullanılır.
    /// </summary>
    public class BlueParaTransactionInfo
    {
        /// <summary>BluePara durumu: PENDING, PROCESSING, BANK_SELECTED, APPROVED, REJECTED, FAILED, TIMEOUT</summary>
        public string Status { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        /// <summary>Yatırımda: oluştururken gönderdiğimiz callback adresi (içinde bizim ref'imiz var)</summary>
        public string? CallbackUrl { get; set; }

        /// <summary>Çekimde: oluştururken gönderdiğimiz merchant_transaction_id</summary>
        public string? MerchantTransactionId { get; set; }
    }
}
