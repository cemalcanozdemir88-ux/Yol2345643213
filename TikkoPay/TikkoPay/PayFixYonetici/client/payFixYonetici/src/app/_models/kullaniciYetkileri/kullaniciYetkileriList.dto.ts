export interface KullaniciYetkileriListDto {
    yetkiId: string,
    kullaniciId: string;
    ustId: string | null;
    siraNumarasi: number;
    baslik: string;
    kullaniciYetkiliMi: boolean
}