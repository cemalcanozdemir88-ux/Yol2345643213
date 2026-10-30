export interface TedarikciCreateDto {
    id: string;
    unvan: string;
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    tedarikcidenAlinanOdemeKomisyonOrani: number;
    yatirimPuani: number;
    hesapNoGosterimSayisi: number;
    bakiyeLimitor: number;
    otoPuan: boolean;
    durum: boolean;
}