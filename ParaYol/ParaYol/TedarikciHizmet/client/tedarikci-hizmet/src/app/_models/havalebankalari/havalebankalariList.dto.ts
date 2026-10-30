export interface HavaleBankalariListDto {
    id: string;
    tedarikciId: string;
    bankaId: string;
    tedarikciAdi: string;
    bankaAdi: string;
    hesapSahibi: string;
    hesapNo: string;
    hesapMinTutar: number;
    hesapMaxTutar: number;
    maxYatirimAdedi: number;
    maxYatirimToplami: number;
    onaylananYatirimAdedi: number;
    onaylananYatirimToplami: number;
    durum: boolean;
}
