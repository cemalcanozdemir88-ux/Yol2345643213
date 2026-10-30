import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { AlertifyService } from '../_services/alertify.service';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTalepleriList.dto';
import { CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';

declare var $: any;

@Component({
  selector: 'app-onayli-cekimler',
  templateUrl: './onayli-cekimler.component.html',
  styleUrls: ['./onayli-cekimler.component.css']
})
export class OnayliCekimlerComponent implements OnInit {

  Authorization = Authorization;

  cekimTalepleri: CekimTalepleriListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  filterModel!: CekimTalepleriPagingFilter;

  toplamTutar: number = 0;
  toplamKomisyon: number = 0;

  activePage: number = 1;
  totalPages: number = 0;

  selectedItemId: string = "";
  public isDeleting: boolean = false;

  constructor(
    private authService: AuthService,
    private cekimTalepleriService: CekimTalepleriService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

    this.filterModel = {
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
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
    if(this.isAdmin()){
      this.getToplamCekimToplami();
    }

    this.getWebSiteleri();
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

  filterChanged(approveDateStart: string, approveDateEnd: string, createdDateStart: string, createdDateEnd: string) {
    this.filterModel.approveDateStart = approveDateStart;
    this.filterModel.approveDateEnd = approveDateEnd;
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    if(this.isAdmin()){
      this.getToplamCekimToplami();
    }
    this.getAll();
  }

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((response) => {
      this.webSiteleriDropDownModel = response.result;
    })
  }

  getToplamCekimToplami(){
    this.cekimTalepleriService.getToplamCekimTutari(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.toplamKomisyon = response.result.toplamKomisyon;
    });
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.cekimTalepleriService.getOnaylananCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.cekimTalepleri = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  downloadExcel() {
    this.cekimTalepleriService.exportToExcel(this.filterModel, true).subscribe((response) => {
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

  toplamCekimTutar(): number {
    return this.cekimTalepleri.reduce((sum, item) => sum + item.tutar, 0);
  }

  toplamKomisyonTutar(): number {
    return this.cekimTalepleri.reduce((sum, item) => sum, 0);
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }

}
