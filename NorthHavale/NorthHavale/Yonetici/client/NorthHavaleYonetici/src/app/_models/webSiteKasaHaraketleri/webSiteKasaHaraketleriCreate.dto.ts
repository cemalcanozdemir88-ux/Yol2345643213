export interface WebSiteKasaHaraketleriCreateDto {
    id: string;
    webSitesiId: string;
    aciklama: string;
    tutar: number;
    dovizKuru: number;
    usdtMiktar: number;
    usdtAdres: string;
    usdtTxtNo: string;
    resimUrl: string;
    islemTipi: number;
    durum: number;
}