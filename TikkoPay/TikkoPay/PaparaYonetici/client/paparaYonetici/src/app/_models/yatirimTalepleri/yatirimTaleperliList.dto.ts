import { IslemTip } from "./islemTip.Enum";
import { YatirimCekimTalepleriDurum } from "./yatirimCekimTalepleriDurum.Enum";

export class YatirimTalepleriListDto {
    id: string = "";
    talepNoWithFormated: string = "";
    bankaId: string = "";
    tedarikciAdi: string = "";
    bankaAdi: string = "";
    webSiteUrl: string = "";
    webSiteTransactionId: string = "";
    adSoyad: string = "";
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
    onayTarihi: Date | undefined;
    guncelleyenKullanici: string = "";
    kayitTarihi: Date = new Date();
    guncellemeTarihi: Date | undefined;
    yonlendirenKullanici: string = "";
    yonlendirilmeTarihi: Date | undefined;
}