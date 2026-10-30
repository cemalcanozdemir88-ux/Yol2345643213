import { Component, OnInit } from '@angular/core';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { AlertifyService } from '../_services/alertify.service';
import { YatirimTalepleriFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { BankaService } from '../_services/banka.service';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTaleperliList.dto';
import { CekimTalepleriFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';

@Component({
  selector: 'app-iptal-talepleri',
  templateUrl: './iptal-talepleri.component.html',
  styleUrls: ['./iptal-talepleri.component.css']
})
export class IptalTalepleriComponent implements OnInit {

  cekimTalepleri: CekimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  filterModel!: CekimTalepleriFilter;

  constructor(
    private cekimTalepleriService: CekimTalepleriService,
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService) {

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: '',
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: ''
    }

  }

  ngOnInit(): void {
    this.getBankalar();
    this.getTedarikciler();
    this.getAll();
  }

  getBankalar() {
    this.bankaService.getAll().subscribe((reponse) => {
      this.bankalarDropDownModel = reponse.result;
    });
  }

  getTedarikciler() {
    this.tedarikcilerService.getAll().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  filterChanged() {
    this.getAll();
  }

  getAll() {
    this.cekimTalepleriService.getRedCekimTalepleri(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.cekimTalepleri = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";

    return "";
  }
}
