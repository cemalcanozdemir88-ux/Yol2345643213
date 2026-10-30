export interface KullanicilarUpdateDto {
    id: string;
    tedarikciId?: string | null;
    webSiteId?: string | null;
    adSoyad: string;
    eposta: string;
    sifre: string;
    kullaniciTipi: number;
    durum: boolean
}