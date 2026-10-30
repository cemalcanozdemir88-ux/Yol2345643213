export interface KullanicilarUpdateDto {
    id: string;
    webSiteId?: string | null;
    affiliateId?: string | null;
    adSoyad: string;
    eposta: string;
    sifre: string;
    kullaniciTipi: number;
    durum: boolean
}