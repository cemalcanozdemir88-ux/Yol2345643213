
export interface KasaHaraketleriVirmanCreateDto {
    
    fromTedarikciId: string;
    fromVirmanIslemTipi: number;

    toTedarikciId: string;
    toVirmanIslemTipi: number;

    tutar: number;
    dovizKuru: number;
    usdtMiktar: number;
    usdtAdres: string;
    usdtTxtNo: string;
    resimUrl: string;
    aciklama: string;
}