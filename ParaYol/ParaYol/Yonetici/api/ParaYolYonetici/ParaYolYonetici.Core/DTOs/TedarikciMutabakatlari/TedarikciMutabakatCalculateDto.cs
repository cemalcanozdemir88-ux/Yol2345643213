namespace ParaYolYonetici.Core.DTOs.TedarikciMutabakatlari
{
    public class TedarikciMutabakatCalculateDto
    {
        public decimal Devir { get; set; }
        public Guid TedarikciId { get; set; }
        public string MutabakatTarihi { get; set; } = string.Empty;
    }
}
