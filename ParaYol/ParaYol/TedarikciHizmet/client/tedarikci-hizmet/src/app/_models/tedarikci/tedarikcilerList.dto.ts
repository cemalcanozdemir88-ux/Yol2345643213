export class TedarikcilerListDto {
    id: string;
    unvan: string = '';
    bankaHavale: boolean;
    kalanLimit: number;
    islemLimit: number;
    gercekBakiye: number;
    bakiyeLimit: number;
    yaririmKomisyonOrani: number;
    cekimKomisyonOrani: number;
    durum: boolean;

    constructor(
        id: string,
        unvan: string,
        bankaHavale: boolean,
        kalanLimit: number,
        islemLimit: number,
        gercekBakiye: number,
        bakiyeLimit: number,
        yaririmKomisyonOrani: number,
        cekimKomisyonOrani: number,
        durum: boolean
    ) {
        this.id = id;
        this.unvan = unvan;
        this.bankaHavale = bankaHavale;
        this.kalanLimit = kalanLimit;
        this.islemLimit = islemLimit;
        this.gercekBakiye = gercekBakiye;
        this.bakiyeLimit = bakiyeLimit;
        this.yaririmKomisyonOrani = yaririmKomisyonOrani;
        this.cekimKomisyonOrani = cekimKomisyonOrani;
        this.durum = durum;
    }
}