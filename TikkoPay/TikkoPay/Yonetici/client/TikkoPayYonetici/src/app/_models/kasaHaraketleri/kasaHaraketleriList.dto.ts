export interface KasaHaraketleriListDto {
    id: string;
    topId: string;
    tedarikciId: string;
    tedarikciAdi: string;
    aciklama: string;
    tutar: number;
    dovizKuru: number;
    usdtMiktar: number;
    usdtAdres: string;
    usdtTxtNo: string;
    resimUrl: string;
    kaydedenKullanici: string;
    kayitTarihi: Date;
    guncelleyenKullanici: string;
    guncellemeTarihi: Date;
    islemTipi: number;
    durum: number;
}