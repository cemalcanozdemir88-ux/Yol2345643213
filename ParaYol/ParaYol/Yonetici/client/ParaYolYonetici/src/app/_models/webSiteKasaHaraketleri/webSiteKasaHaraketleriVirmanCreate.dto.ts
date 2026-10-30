export interface WebSiteKasaHaraketleriVirmanCreateDto {
    
    fromWebSitesiId: string;
    fromVirmanIslemTipi: number;

    toWebSitesiId: string;
    toVirmanIslemTipi: number;

    tutar: number;
    dovizKuru: number;
    usdtMiktar: number;
    usdtAdres: string;
    usdtTxtNo: string;
    resimUrl: string;
    aciklama: string;
}