import { Component, OnInit } from '@angular/core';
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
  selector: 'app-iptal-cekimler',
  templateUrl: './iptal-cekimler.component.html',
  styleUrls: ['./iptal-cekimler.component.css']
})
export class IptalCekimlerComponent {

  Authorization = Authorization;

  cekimTalepleri: CekimTalepleriListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  filterModel!: CekimTalepleriPagingFilter;
  selectedCancelDescription: string = "";

  activePage: number = 1;
  totalPages: number = 0;

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

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((response) => {
      this.webSiteleriDropDownModel = response.result;
    })
  }

  filterChanged(approveDateStart: string, approveDateEnd: string, createdDateStart: string, createdDateEnd: string) {
    this.filterModel.approveDateStart = approveDateStart;
    this.filterModel.approveDateEnd = approveDateEnd;
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.cekimTalepleriService.getRedCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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
    this.cekimTalepleriService.exportToExcel(this.filterModel, false).subscribe((response) => {
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

  startOpenCancelDescriptionModal(cancelDescription: string) {
    this.selectedCancelDescription = cancelDescription;
  }

  closeCancelDescriptionModal() {
    this.selectedCancelDescription = "";
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }
  
}
