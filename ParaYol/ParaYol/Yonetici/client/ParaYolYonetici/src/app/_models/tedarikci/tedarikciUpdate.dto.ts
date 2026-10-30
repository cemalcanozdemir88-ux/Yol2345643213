export interface TedarikciCreateDto {
    id: string;
    unvan: string;
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    tedarikcidenAlinanOdemeKomisyonOrani: number;
    bakiyeLimitor: number;
    otoPuan: boolean;
    durum: boolean;
}