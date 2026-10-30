export interface WebSiteKullanicilariUpdateDto {
    id: string;
    webSiteId: string;
    adSoyad: string;
    eposta: string;
    sifre: string;
    kullaniciTipi: number;
    durum: boolean
}