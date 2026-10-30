export interface KullanicilarListDto {
    id: string;
    webSiteId?: string | null;
    webSiteUrl?: string | null;
    affiliateId?: string | null;
    affiliateUnvan?: string | null;
    adSoyad: string;
    eposta: string;
    kullaniciTipi: number;
    kullaniciTipiAdi: string;
    durum: boolean
}