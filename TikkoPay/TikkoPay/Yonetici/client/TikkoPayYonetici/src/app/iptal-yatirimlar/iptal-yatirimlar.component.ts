import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { AlertifyService } from '../_services/alertify.service';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { BankaService } from '../_services/banka.service';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { Durum } from '../_enums/durum.enum';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { NgForm } from '@angular/forms';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';

declare var $: any;

@Component({
  selector: 'app-iptal-yatirimlar',
  templateUrl: './iptal-yatirimlar.component.html',
  styleUrls: ['./iptal-yatirimlar.component.css']
})
export class IptalYatirimlarComponent {

  Authorization = Authorization;

  @ViewChild('bekleyeneAlModalKapatButton') bekleyeneAlModalKapatButton!: ElementRef;
  @ViewChild('yenidenYonlendirModalKapatButton') yenidenYonlendirModalKapatButton!: ElementRef;

  selectedYonlendirItem: YatirimTalepleriListDto | null = null;
  yonlendirModel: { tedarikciId: string, tutar: number | null } = { tedarikciId: '', tutar: null };

  public isLoading: boolean = false;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  selectedCancelDescription: string = "";

  constructor(
    private authService: AuthService,
    private yatirimTalepleriService: YatirimTalepleriService,
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
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
      this.getBankalar();
      this.getTedarikciler();
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

  getBankalar() {
    this.bankaService.getAll().subscribe((reponse) => {
      this.bankalarDropDownModel = reponse.result;
    });
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
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

    this.yatirimTalepleriService.getRedYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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
    this.yatirimTalepleriService.exportToExcel(this.filterModel, IslemTip.Yatirim, Durum.Red).subscribe((response) => {
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

  showCancelModal(event: any){
    alert("test")
  }

  startOpenCancelDescriptionModal(cancelDescription: string) {
    this.selectedCancelDescription = cancelDescription;
  }

  closeCancelDescriptionModal() {
    this.selectedCancelDescription = "";
  }

  startBekleyeneAl(itemId: string) {
    this.selectedItemId = itemId;
  }

  closeBekleyeneAlModalKapat() {
    this.selectedItemId = "";
    this.bekleyeneAlModalKapatButton.nativeElement.click();
  }

  submitBekleyeneAlForm(bekleyeneAlForm: NgForm){
    if (!this.selectedItemId) {
      this.alertify.error("Yatırım seçiniz!");
      return;
    }

    this.isLoading = true;

    this.yatirimTalepleriService.bekleyeneAl(this.selectedItemId).subscribe({
      next: response => {
        if (response.success) {
          this.closeBekleyeneAlModalKapat();
          this.getAll();
          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }
        this.isLoading = false;
      },
      error: error => {
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });

  }

  startYenidenYonlendir(item: YatirimTalepleriListDto) {
    this.selectedYonlendirItem = item;
    this.yonlendirModel = { tedarikciId: item.tedarikciId ?? '', tutar: item.tutar };
  }

  closeYenidenYonlendirModal() {
    this.selectedYonlendirItem = null;
    this.yonlendirModel = { tedarikciId: '', tutar: null };
    this.yenidenYonlendirModalKapatButton.nativeElement.click();
  }

  submitYenidenYonlendirForm() {
    if (!this.selectedYonlendirItem) {
      this.alertify.error("Yatırım seçiniz!");
      return;
    }
    if (!this.yonlendirModel.tedarikciId) {
      this.alertify.error("Tedarikçi seçiniz!");
      return;
    }
    if (!this.yonlendirModel.tutar || this.yonlendirModel.tutar <= 0) {
      this.alertify.error("Geçerli bir tutar giriniz!");
      return;
    }

    this.isLoading = true;

    this.yatirimTalepleriService.yenidenYonlendir(this.selectedYonlendirItem.id, this.yonlendirModel.tedarikciId, this.yonlendirModel.tutar).subscribe({
      next: response => {
        if (response.success) {
          this.closeYenidenYonlendirModal();
          this.getAll(this.activePage);
          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }
        this.isLoading = false;
      },
      error: error => {
        this.alertify.error(error?.error?.message ?? "Hata oluştu");
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }

  formatDateToSlash(date: Date): string {
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\./g, '/');
  }
}
