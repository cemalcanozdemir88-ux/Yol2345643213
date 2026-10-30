export interface TedarikciMutabakatlariListDto {
    id: string;
    tedarikciId: string;
    tedarikciAdi: string;
    devir: number;
    toplamYatirim: number;
    toplamYatirimKomisyon: number;
    toplamCekim: number;
    toplamCekimKomisyon: number;
    toplamGiris: number;
    toplamGirisKomisyon: number;
    toplamCikis: number;
    toplamGirisManual: number;
    toplamCikisManual: number;
    toplamVirmanGiris: number;
    toplamVirmanCikis: number;
    kasa: number;
    mutabakatTarihi: Date;
}   
