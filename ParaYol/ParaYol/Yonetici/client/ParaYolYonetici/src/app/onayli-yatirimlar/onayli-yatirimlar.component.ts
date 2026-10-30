import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from '../_services/alertify.service';
import { BankaService } from '../_services/banka.service';
import { TedarikciService } from '../_services/tedarikci.service';
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
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';
import { HavaleBankalariService } from '../_services/havalebankalari.service';
import { WebSitePaymentMethodsDto } from '../_models/webSitePaymentMethods/webSitePaymentMethods.dto';
import { WebSitePaymentMethodsService } from '../_services/webSitePaymentMethods.service';

declare var $: any;

@Component({
  selector: 'app-onayli-yatirimlar',
  templateUrl: './onayli-yatirimlar.component.html',
  styleUrls: ['./onayli-yatirimlar.component.css']
})
export class OnayliYatirimlarComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('manuelYatirimEkleModalButton') manuelYatirimEkleModalButton!: ElementRef;
  @ViewChild('manuelYatirimEkleModalKapatButton') manuelYatirimEkleModalKapatButton!: ElementRef;
  @ViewChild('yatirimSilModalKapatButton') yatirimSilModalKapatButton!: ElementRef;

  havaleBankaDropDownModel: HavaleBankalariListDto[] = [];
  paymentMethodsDropDownModel: WebSitePaymentMethodsDto[] = [];

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  yatirimCreateModel!: YatirimCreateDto;
  isHavaleBankalariLoading = false;
  isPaymentMethodsLoading = false;
  isManuelYatirimCreating = false;

  filterModel!: YatirimTalepleriPagingFilter;

  toplamTutar: number = 0;
  tedarikciToplamKomisyon: number = 0;
  websiteToplamKomisyon: number = 0;

  ortalamSureText: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  selectedItemId: string = "";
  public isDeleting: boolean = false;

  constructor(
    private authService: AuthService,
    private yatirimTalepleriService: YatirimTalepleriService,
    private bankaService: BankaService,
    private havaleBankalariService: HavaleBankalariService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriService: WebSiteleriService,
    private webSitePaymentMethodsService: WebSitePaymentMethodsService,
    private alertify: AlertifyService) {

    this.createEmptyYatirimCreateModel();

    const today = new Date(); 
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1); 
  
    const formatDate = (date: Date) => {
      return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
    }

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: formatDate(oneMonthAgo),
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    if (this.isAdmin()) {
      this.getBankalar();
      this.getTedarikciler();
      this.getToplamYatirimToplami();
      this.getOrtalamaOnaySuresi();
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
      this.getOrtalamaOnaySuresi();
    }

    this.getAll();
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

  getToplamYatirimToplami() {
    this.yatirimTalepleriService.getToplamYatirimTutari(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.tedarikciToplamKomisyon = response.result.tedarikciToplamKomisyon;
      this.websiteToplamKomisyon = response.result.websiteToplamKomisyon;
    });
  }

  getOrtalamaOnaySuresi() {
    this.yatirimTalepleriService.getOrtalamaOnaySuresi(this.filterModel).subscribe((response) => {
      this.ortalamSureText = response.result.formatliSure;
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

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";

    return "";
  }

  toplamYatirimTutar(): number {
    return this.yatirimTalepleri.reduce((sum, item) => sum + item.tutar, 0);
  }
  toplamKomisyonTutar(): number {
    return this.yatirimTalepleri.reduce((sum, item) => sum + item.tedarikciKomisyon, 0);
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }


  createEmptyYatirimCreateModel() {
    this.yatirimCreateModel = {
      tedarikciId: '',
      havaleBankaId: '',
      webSiteId: '',
      paymentMethodId: '',
      adSoyad: '',
      userName: '',
      userId: '',
      durum: Durum.Onaylandi,
      islemTipi: IslemTip.Yatirim,
      tutar: 0,
    };
  }

  openManuelYatirimEkleModal() {
    this.createEmptyYatirimCreateModel();
    this.manuelYatirimEkleModalButton.nativeElement.click();
  }

  closeManuelYatirimEkleModal(){
    if (this.manuelYatirimEkleModalKapatButton && this.manuelYatirimEkleModalKapatButton.nativeElement) {
      this.manuelYatirimEkleModalKapatButton.nativeElement.click();
    }

    this.createEmptyYatirimCreateModel();
  }

  closeManuelYatirimEkleModalKapat() {
    this.createEmptyYatirimCreateModel();
  }

  tedarikciChanged(event: any): void{
    this.isHavaleBankalariLoading = true;
    const selectedTedarikciId = event.target.value;
    this.havaleBankalariService.getByTedarikciId(selectedTedarikciId).subscribe((reponse) => {
      this.havaleBankaDropDownModel = reponse.result;
      this.isHavaleBankalariLoading = false;
    });
  }

  webSiteChanged(event: any): void {
    
    this.yatirimCreateModel.paymentMethodId = '';

    this.isPaymentMethodsLoading = true;
    const selectedWebSiteId = event.target.value;

    this.webSitePaymentMethodsService.getByWebSiteId(selectedWebSiteId).subscribe((response) => {
      this.paymentMethodsDropDownModel = response.result;
      this.isPaymentMethodsLoading = false;
    });
  }

  submitManuelYatirimEkleForm(manuelYatirimEkleForm: NgForm) {
    if (!manuelYatirimEkleForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isManuelYatirimCreating = true;

    this.yatirimTalepleriService.createYatirim(this.yatirimCreateModel).subscribe({
      next: response => {
        this.isManuelYatirimCreating = false;

        if (response.success) {
          this.closeManuelYatirimEkleModal();
          if (this.isAdmin()) {
            this.getToplamYatirimToplami();
            this.getOrtalamaOnaySuresi();
          }
          this.getAll();
          this.alertify.success("Kayıt eklendi");
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isManuelYatirimCreating = false;
      },
      complete: () => {

      }
    });

  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
  }

  cancelDelete() {
    this.selectedItemId = "";
  }

  closeYatirimSilModal() {
    if (this.yatirimSilModalKapatButton && this.yatirimSilModalKapatButton.nativeElement) {
      this.yatirimSilModalKapatButton.nativeElement.click();
    }
  }

  deleteRecord(){
    if (this.selectedItemId !== "") {
      this.isDeleting = true;
      this.yatirimTalepleriService.delete(this.selectedItemId).subscribe({
        next: (response) => {
          if (this.isAdmin()) {
            this.getToplamYatirimToplami();
            this.getOrtalamaOnaySuresi();
          }
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          }
          else {
            this.alertify.error(response.message);
          }
        },
        error: error => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeYatirimSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeYatirimSilModal();
        }
      });
    }
  }

  formatDateToSlash(date: Date): string {
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\./g, '/');
  }

}
