export interface TedarikciBakiyeListDto {
    id: string;
    unvan: string;
    tedarikciDurum: boolean;
    bakiyeLimitor: number;
    toplamYatirim: number;
    toplamYatirimKomisyon: number;
    toplamCekim: number;
    toplamCekimKomisyon: number;
    toplamKasaHaraketiGiris: number;
    toplamKasaHaraketiCikis: number;
}