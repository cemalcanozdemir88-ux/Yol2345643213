export interface KullanicilarCreateDto {
    id: string;
    webSiteId?: string | null;
    affiliateId?: string | null;
    adSoyad: string;
    eposta: string;
    sifre: string;
    kullaniciTipi: number;
    isOtpEnabled: boolean;
    durum: boolean;
}