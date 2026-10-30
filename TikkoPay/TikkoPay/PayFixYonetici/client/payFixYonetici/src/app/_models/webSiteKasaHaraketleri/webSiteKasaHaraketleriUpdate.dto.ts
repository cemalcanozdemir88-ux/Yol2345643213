export interface WebSiteKasaHaraketleriUpdateDto {
    id: string;
    webSitesiId: string;
    aciklama: string;
    tutar: number;
    usdtMiktar: number;
    usdtAdres: string;
    usdtTxtNo: string;
    resimUrl: string;
    islemTipi: number;
    durum: number;
}