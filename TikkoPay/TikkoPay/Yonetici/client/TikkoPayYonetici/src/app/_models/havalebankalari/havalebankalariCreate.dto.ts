export interface HavaleBankalariCreateDto {
    id: string;
    tedarikciId: string;
    bankaId: string;
    hesapSahibi: string;
    hesapNo: string;
    hesapMinTutar: number;
    hesapMaxTutar: number;
    maxYatirimAdedi: number;
    maxYatirimToplami: number;
    onaylananYatirimAdediTemp: number;
    durum: boolean;
}