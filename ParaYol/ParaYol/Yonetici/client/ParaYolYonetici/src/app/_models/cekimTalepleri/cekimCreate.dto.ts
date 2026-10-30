export interface CekimCreateDto {
    tedarikciId: string | null;
    webSiteId: string;
    adSoyad: string;
    hesapNo: string;
    hesapSahibi: string;
    userName: string;
    userId: string;
    iptalAciklama: string;
    durum: number;
    islemTipi: number;
    tutar: number;
}