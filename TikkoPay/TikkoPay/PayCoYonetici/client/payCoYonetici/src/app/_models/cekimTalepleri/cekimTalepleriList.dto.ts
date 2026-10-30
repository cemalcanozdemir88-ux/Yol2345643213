export class CekimTalepleriListDto {
    id: string = "";
    talepNoWithFormated: string = "";
    webSiteUrl: string = "";
    webSiteTransactionId: string = "";
    adSoyad: string = "";
    userName: string = "";
    userId: string = "";
    tutar: number = 0;
    toplam: number = 0;
    iptalAciklama: string = "";
    onayTarihi: Date | undefined;
    kayitTarihi: Date = new Date();
    guncellemeTarihi: Date | undefined;
}