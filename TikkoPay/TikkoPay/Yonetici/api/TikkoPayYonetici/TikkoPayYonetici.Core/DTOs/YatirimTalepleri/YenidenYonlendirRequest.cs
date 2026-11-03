namespace TikkoPayYonetici.Core.DTOs.YatirimTalepleri
{
    /// <summary>
    /// İptal edilen bir yatırımı, (isteğe bağlı olarak) farklı tutar ve/veya tedarikçi ile
    /// yeni bir kayıt olarak tekrar tedarikçi onayına göndermek için kullanılır.
    /// Mevcut kayıt değiştirilmez, geçmiş (history) olarak saklanır.
    /// </summary>
    public class YenidenYonlendirRequest
    {
        public Guid Id { get; set; }
        public Guid TedarikciId { get; set; }
        public decimal Tutar { get; set; }
    }
}
