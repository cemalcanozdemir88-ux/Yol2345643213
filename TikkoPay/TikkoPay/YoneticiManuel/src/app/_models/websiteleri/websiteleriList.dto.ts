export interface WebSiteleriListDto {
    id: string;
    webSiteUrl: string;
    baslik: string;
    kisaBaslik: string;
    affiliateId: string;
    affiliateYaririmKomisyonOrani: number | null;
    affiliateCekimKomisyonOrani: number | null;
    affiliateUnvan: string;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    odemeKomisyonOrani: number;
    apiClientId: string;
    apiClientSecret: string;
    type: number;
    bakiyeLimitor: number;
    minYatirimTutari: number;
}