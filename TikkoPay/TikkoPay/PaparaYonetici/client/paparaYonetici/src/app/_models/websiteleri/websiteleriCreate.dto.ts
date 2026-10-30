export interface WebSiteleriCreateDto {
    id: string;
    webSiteUrl: string;
    affiliateId: string | null;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    odemeKomisyonOrani: number;
    apiClientId: string;
    apiClientSecret: string;
    depositCallbackUrl: string;
    withdrawCallbackUrl: string;
    type: number;
}