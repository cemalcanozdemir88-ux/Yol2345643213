import { IslemTip } from "../yatirimTalepleri/islemTip.Enum";
import { YatirimCekimTalepleriDurum } from "../yatirimTalepleri/yatirimCekimTalepleriDurum.Enum";

export class CekimTalepleriListDto {
    id: string = "";
    talepNoWithFormated: string = "";
    paymentMethodId: number = 0;
    bankaId: string = "";
    tedarikciId: string = "";
    tedarikciAdi: string = "";
    bankaAdi: string = "";
    webSiteUrl: string = "";
    webSiteTransactionId: string = "";
    withdrawTransactionId: string = "";
    adSoyad: string = "";
    userName: string = "";
    userId: string = "";
    yontemTipi: string = "";
    hesapSahibi: string = "";
    hesapNo: string = "";
    hesapNoBanka: string = "";
    tutar: number = 0;
    tedarikciKomisyon: number = 0;
    toplam: number = 0;
    islemTipi: IslemTip = IslemTip.Yatirim;
    iptalAciklama: string = "";
    dekontKontrolDurumu: number = 0;
    durum: YatirimCekimTalepleriDurum = YatirimCekimTalepleriDurum.Bekliyor;
    cokluTedarikciyeYonlendirildi: boolean = false;
    onayTarihi: Date | undefined;
    guncelleyenKullanici: string = "";
    kayitTarihi: Date = new Date();
    guncellemeTarihi: Date | undefined;
    yonlendirenKullanici: string = "";
    yonlendirilmeTarihi: Date | undefined;
}