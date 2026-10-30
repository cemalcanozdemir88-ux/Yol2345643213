import { IslemTip } from "./islemTip.Enum";
import { YatirimCekimTalepleriDurum } from "./yatirimCekimTalepleriDurum.Enum";

export class YatirimTalepleriListDto {
    id: string = "";
    talepNoWithFormated: string = "";
    paymentMethodId: number = 0;
    bankaId: string = "";
    tedarikciId: string = "";
    tedarikciAdi: string = "";
    bankaAdi: string = "";
    webSiteUrl: string = "";
    webSiteTransactionId: string = "";
    depositTransactionId: string = "";
    adSoyad: string = "";
    adSoyadYatirimYapan: string = "";
    userName: string = "";
    userId: string = "";
    yontemTipi: string = "";
    hesapSahibi: string = "";
    hesapNo: string = "";
    tutar: number = 0;
    tedarikciKomisyon: number = 0;
    toplam: number = 0;
    islemTipi: IslemTip = IslemTip.Yatirim;
    iptalAciklama: string = "";
    durum: YatirimCekimTalepleriDurum = YatirimCekimTalepleriDurum.Bekliyor;
    cokluTedarikciyeYonlendirildi: boolean = false;
    onayTarihi: Date | undefined;
    guncelleyenKullanici: string = "";
    kayitTarihi: Date = new Date();
    guncellemeTarihi: Date | undefined;
    yonlendirenKullanici: string = "";
    yonlendirilmeTarihi: Date | undefined;
}