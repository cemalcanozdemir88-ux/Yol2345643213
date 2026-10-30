export interface WebSiteMutabakatlariListDto {
    id: string;
    webSiteId: string;
    webSiteAdi: string;
    devir: number;
    toplamYatirim: number;
    toplamYatirimKomisyon: number;
    toplamCekim: number;
    toplamCekimKomisyon: number;
    toplamGiris: number;
    toplamCikis: number;
    toplamCikisKomisyon: number;
    toplamGirisManual: number;
    toplamCikisManual: number;
    toplamVirmanGiris: number;
    toplamVirmanCikis: number;
    kasa: number;
    mutabakatTarihi: Date;
}