namespace ParaYolYonetici.Core.DTOs.CekimTalepleri
{
    public class MultiTedarikciYonlendirmeDto
    {
        public Guid CekimTalepleriId { get; set; }
        public List<TedarikciWithAmount> TedarikciWithAmount { get; set; } = new();
    }

    public class TedarikciWithAmount
    {
        public Guid TedarikciId { get; set; }
        public decimal Amount { get; set; }
    }
}
