export interface KasaHaraketleriUpdateDto {
    id: string;
    tedarikciId: string;
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