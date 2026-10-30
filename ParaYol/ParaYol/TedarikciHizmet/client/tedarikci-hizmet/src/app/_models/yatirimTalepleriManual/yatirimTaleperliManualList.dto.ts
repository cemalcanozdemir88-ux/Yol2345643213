import { IslemTip } from 'src/app/_enums/islemTip.Enum';
import { YatirimCekimTalepleriDurum } from 'src/app/_enums/yatirimCekimTalepleriDurum.Enum';

export class YatirimTalepleriManualListDto {
  id: string = '';
  talepNoWithFormated: string = '';
  bankaId: string = '';
  tedarikciAdi: string = '';
  bankaAdi: string = '';
  adSoyad: string = '';
  yontemTipi: string = '';
  hesapSahibi: string = '';
  hesapNo: string = '';
  hesapNoBanka: string = '';
  tutar: number = 0;
  tedarikciKomisyon: number = 0;
  toplam: number = 0;
  islemTipi: IslemTip = IslemTip.Yatirim;
  iptalAciklama: string = "";
  durum: YatirimCekimTalepleriDurum = YatirimCekimTalepleriDurum.Bekliyor;
  onayTarihi: Date | undefined;
  guncelleyenKullanici: string = '';
  kayitTarihi: Date = new Date();
}
