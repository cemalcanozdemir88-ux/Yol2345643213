export class TedarikcilerListDto {
    id: string;
    unvan: string = '';
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    yatirimPuani: number;
    hesapNoGosterimSayisi: number;
    bakiyeLimitor: number;
    durum: boolean;

    constructor(
        id: string,
        unvan: string,
        bankaHavale: boolean,
        yaririmKomisyonOrani: number,
        cekimKomisyonOrani: number,
        yatirimPuani: number,
        hesapNoGosterimSayisi: number,
        bakiyeLimitor: number,
        durum: boolean
    ) {
        this.id = id;
        this.unvan = unvan;
        this.bankaHavale = bankaHavale;
        this.yaririmKomisyonOrani = yaririmKomisyonOrani;
        this.cekimKomisyonOrani = cekimKomisyonOrani;
        this.yatirimPuani = yatirimPuani;
        this.hesapNoGosterimSayisi = hesapNoGosterimSayisi;
        this.bakiyeLimitor = bakiyeLimitor;
        this.durum = durum;
    }
}