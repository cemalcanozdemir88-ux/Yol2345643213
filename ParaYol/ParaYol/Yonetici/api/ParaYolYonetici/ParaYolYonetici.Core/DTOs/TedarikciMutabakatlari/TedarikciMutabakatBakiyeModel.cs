namespace ParaYolYonetici.Core.DTOs.TedarikciMutabakatlari
{
    public class TedarikciMutabakatBakiyeModel
    {
        public decimal ToplamYatirim { get; set; }
        public decimal ToplamYatirimKomisyon { get; set; }
        public decimal ToplamCekim { get; set; }
        public decimal ToplamCekimKomisyon { get; set; }
        public decimal ToplamGiris { get; set; }
        public decimal ToplamGirisKomisyon { get; set; }
        public decimal ToplamCikis { get; set; }
        public decimal ToplamGirisManuel { get; set; }
        public decimal ToplamCikisManuel { get; set; }
        public decimal ToplamVirmanGiris { get; set; }
        public decimal ToplamVirmanCikis { get; set; }
    }
}
