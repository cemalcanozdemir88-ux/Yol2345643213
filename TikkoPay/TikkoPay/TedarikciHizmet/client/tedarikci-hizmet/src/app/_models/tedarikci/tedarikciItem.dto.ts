export interface TedarikciItemDto {
    id: string;
    unvan: string;
    bankaHavale: boolean;
    kalanLimit: number;
    islemLimit: number;
    gercekBakiye: number;
    bakiyeLimit: number;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    durum: boolean;
    eposta: string;
    sifre: string;
}