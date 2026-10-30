namespace TedarikciHizmet.Core.DTOs.TedarikciMutabakatlari
{
    public class TedarikciMutabakatlariListDto
    {
        public Guid Id { get; set; }
        public Guid TedarikciId { get; set; }
        public string TedarikciAdi { get; set; } = string.Empty;
        public decimal Devir { get; set; }
        public decimal ToplamYatirim { get; set; }
        public decimal ToplamYatirimKomisyon { get; set; }
        public decimal ToplamCekim { get; set; }
        public decimal ToplamCekimKomisyon { get; set; }
        public decimal ToplamGiris { get; set; }
        public decimal ToplamGirisKomisyon { get; set; }
        public decimal ToplamCikis { get; set; }
        public decimal ToplamGirisManual { get; set; }
        public decimal ToplamCikisManual { get; set; }
        public decimal ToplamVirmanGiris { get; set; }
        public decimal ToplamVirmanCikis { get; set; }
        public decimal Kasa { get; set; }
        public string MutabakatTarihi { get; set; } = string.Empty;
    }
}
