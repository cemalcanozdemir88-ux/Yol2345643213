export interface KullanicilarCreateDto {
    id: string;
    tedarikciId?: string | null;
    webSiteId?: string | null;
    affiliateId?: string | null;
    adSoyad: string;
    eposta: string;
    sifre: string;
    kullaniciTipi: number;
    ipAdresSayisi: number;
    isOtpEnabled: boolean;
    durum: boolean;
}