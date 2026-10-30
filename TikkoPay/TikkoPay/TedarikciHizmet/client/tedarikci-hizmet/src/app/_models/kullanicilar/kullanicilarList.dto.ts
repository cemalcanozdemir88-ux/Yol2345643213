export interface KullanicilarListDto {
    id: string;
    tedarikciId?: string | null;
    tedarikciAdi?: string | null;
    webSiteId?: string | null;
    webSiteUrl?: string | null;
    adSoyad: string;
    eposta: string;
    kullaniciTipi: number;
    kullaniciTipiAdi: string;
    durum: boolean
}