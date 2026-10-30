import { KullaniciIpAdresleriListDto } from "./kullaniciIpAdresleriList.dto";

export interface KullaniciIpAdresleriWithAvailableCount {
    ipAdresSayisi: number;
    kullaniciIpAdresleriListDtos: KullaniciIpAdresleriListDto[];
}
