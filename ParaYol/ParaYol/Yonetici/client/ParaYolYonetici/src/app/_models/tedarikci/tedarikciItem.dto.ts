export interface TedarikciItemDto {
    id: string;
    unvan: string;
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    tedarikcidenAlinanOdemeKomisyonOrani: number;
    otoPuan: boolean;
    durum: boolean;
    eposta: string;
    sifre: string;
}