import { Component } from '@angular/core';
import { RecordsPerPage } from 'src/app/_constants/constants';
import { Durum } from 'src/app/_enums/durum.enum';
import { IslemTip } from 'src/app/_enums/islemTip.Enum';
import { YatirimCekimTalepleriDurum } from 'src/app/_enums/yatirimCekimTalepleriDurum.Enum';
import { BankaListDto } from 'src/app/_models/banka/bankaList.dto';
import { TedarikcilerListDto } from 'src/app/_models/tedarikci/tedarikcilerList.dto';
import { YatirimTalepleriListDto } from 'src/app/_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriPagingFilter } from 'src/app/_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BankaService } from 'src/app/_services/banka.service';
import { YatirimTalepleriService } from 'src/app/_services/yatirimTalepleri.service';

declare var $: any;

@Component({
  selector: 'app-onaylanan-yatirimlar',
  templateUrl: './onaylanan-yatirimlar.component.html',
  styleUrls: ['./onaylanan-yatirimlar.component.css']
})
export class OnaylananYatirimlarComponent {
  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  toplamTutar: number = 0;
  toplamKomisyon: number = 0;

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private yatirimTalepleriService: YatirimTalepleriService,
    private bankaService: BankaService,
    private alertify: AlertifyService) {

    this.filterModel = {
      bankaId: '',
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    this.getBankalar();
    this.getToplamYatirimToplami();
    this.getAll();
  }

  ngAfterViewInit() {

    $('.dateControl').datepicker({
      language: 'tr',
      keyboardNavigation: false,
      forceParse: false,
      calendarWeeks: true,
      autoclose: true,
      format: "dd/mm/yyyy"
    });

  }

  getBankalar() {
    this.bankaService.getAll().subscribe((response) => {
      this.bankalarDropDownModel = response.result;
    });
  }

  getToplamYatirimToplami(){
    this.yatirimTalepleriService.getToplamYatirimTutariWithFilter(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.toplamKomisyon = response.result.toplamKomisyon;
    });
  }

  filterChanged(approveDateStart: string, approveDateEnd: string, createdDateStart: string, createdDateEnd: string) {
    this.filterModel.approveDateStart = approveDateStart;
    this.filterModel.approveDateEnd = approveDateEnd;
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    this.getToplamYatirimToplami();
    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getOnaylananYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.yatirimTalepleri = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }


  downloadExcel() {
    this.yatirimTalepleriService.exportToExcel(this.filterModel, IslemTip.Yatirim, Durum.Onaylandi).subscribe((response) => {
      if (response.success) {

        var binaryData = atob(response.result.base64String);

        const blob = new Blob([new Uint8Array(binaryData.length).map((_, index) => binaryData.charCodeAt(index))], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = response.result.fileName;
        a.click();

        window.URL.revokeObjectURL(url);

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
