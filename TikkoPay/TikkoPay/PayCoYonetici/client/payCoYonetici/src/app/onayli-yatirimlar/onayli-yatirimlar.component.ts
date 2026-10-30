import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from '../_services/alertify.service';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { Durum } from '../_enums/durum.enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { NgForm } from '@angular/forms';
import { YatirimCreateDto } from '../_models/yatirimTalepleri/yatirimCreate.dto';

declare var $: any;

@Component({
  selector: 'app-onayli-yatirimlar',
  templateUrl: './onayli-yatirimlar.component.html',
  styleUrls: ['./onayli-yatirimlar.component.css']
})
export class OnayliYatirimlarComponent implements OnInit {

  Authorization = Authorization;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  yatirimCreateModel!: YatirimCreateDto;
  isManuelYatirimCreating = false;

  filterModel!: YatirimTalepleriPagingFilter;

  toplamTutar: number = 0;
  toplamKomisyon: number = 0;

  activePage: number = 1;
  totalPages: number = 0;

  selectedItemId: string = "";
  public isDeleting: boolean = false;

  constructor(
    private authService: AuthService,
    private yatirimTalepleriService: YatirimTalepleriService,
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
    if (this.isAdmin()) {
      this.getToplamYatirimToplami();
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

    if (this.isAdmin()) {
      this.getToplamYatirimToplami();
    }

    this.getAll();
  }

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((response) => {
      this.webSiteleriDropDownModel = response.result;
    })
  }

  getToplamYatirimToplami() {
    this.yatirimTalepleriService.getToplamYatirimTutari(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.toplamKomisyon = response.result.toplamKomisyon;
    });
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

  toplamYatirimTutar(): number {
    return this.yatirimTalepleri.reduce((sum, item) => sum + item.tutar, 0);
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }
}
