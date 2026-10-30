export class TedarikcilerListDto {
    id: string;
    unvan: string = '';
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    tedarikcidenAlinanOdemeKomisyonOrani: number;
    yatirimPuani: number;
    hesapNoGosterimSayisi: number;
    fastHesapSayisi: number;
    anlikBakiye: number;
    bakiyeLimitor: number;
    otoPuan: boolean;
    durum: boolean;

    constructor(
        id: string,
        unvan: string,
        bankaHavale: boolean,
        yaririmKomisyonOrani: number,
        cekimKomisyonOrani: number,
        tedarikcidenAlinanOdemeKomisyonOrani: number,
        yatirimPuani: number,
        hesapNoGosterimSayisi: number,
        fastHesapSayisi: number,
        anlikBakiye: number,
        bakiyeLimitor: number,
        otoPuan: boolean,
        durum: boolean
    ) {
        this.id = id;
        this.unvan = unvan;
        this.bankaHavale = bankaHavale;
        this.yaririmKomisyonOrani = yaririmKomisyonOrani;
        this.cekimKomisyonOrani = cekimKomisyonOrani;
        this.tedarikcidenAlinanOdemeKomisyonOrani = tedarikcidenAlinanOdemeKomisyonOrani;
        this.yatirimPuani = yatirimPuani;
        this.hesapNoGosterimSayisi = hesapNoGosterimSayisi;
        this.fastHesapSayisi = fastHesapSayisi;
        this.anlikBakiye = anlikBakiye;
        this.bakiyeLimitor = bakiyeLimitor;
        this.otoPuan = otoPuan;
        this.durum = durum;
    }
}