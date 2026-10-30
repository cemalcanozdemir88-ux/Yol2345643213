export class TedarikcilerListDto {
    id: string;
    unvan: string = '';
    bankaHavale: boolean;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    yatirimPuani: number;
    durum: boolean;

    constructor(
        id: string,
        unvan: string,
        bankaHavale: boolean,
        yaririmKomisyonOrani: number,
        cekimKomisyonOrani: number,
        yatirimPuani: number,
        durum: boolean
    ) {
        this.id = id;
        this.unvan = unvan;
        this.bankaHavale = bankaHavale;
        this.yaririmKomisyonOrani = yaririmKomisyonOrani;
        this.cekimKomisyonOrani = cekimKomisyonOrani;
        this.yatirimPuani = yatirimPuani;
        this.durum = durum;
    }
}