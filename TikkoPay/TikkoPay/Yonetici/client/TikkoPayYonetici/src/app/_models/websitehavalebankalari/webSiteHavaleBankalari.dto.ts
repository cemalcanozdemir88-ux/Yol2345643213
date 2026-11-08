export interface WebSiteHavaleBankalariListDto {
    id: string;
    webSiteId: string;
    havaleBankaId: string;
    tedarikciId: string;
    tedarikciBaslik: string;
    bankaBaslik: string;
    hesapSahibi: string;
    hesapNo: string;
    hesapMinTutar: number;
    hesapMaxTutar: number;
    durum: boolean;
}

export interface WebSiteHavaleBankalariCreateDto {
    webSiteId: string;
    havaleBankaId: string;
}
