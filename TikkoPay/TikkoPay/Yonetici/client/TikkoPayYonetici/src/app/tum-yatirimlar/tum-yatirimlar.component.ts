import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
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
import { TumYatirimTalepleriFilter } from '../_models/yatirimTalepleri/tumYatirimTalepleriFilter';
import { WebSitePaymentMethodsDto } from '../_models/webSitePaymentMethods/webSitePaymentMethods.dto';
import { WebSitePaymentMethodsService } from '../_services/webSitePaymentMethods.service';

declare var $: any;

@Component({
  selector: 'app-tum-yatirimlar',
  templateUrl: './tum-yatirimlar.component.html',
  styleUrls: ['./tum-yatirimlar.component.css']
})
export class TumYatirimlarComponent implements OnInit {

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

  filterModel!: TumYatirimTalepleriFilter;

  toplamTutar: number = 0;
  tedarikciToplamKomisyon: number = 0;
  websiteToplamKomisyon: number = 0;

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
      webSiteId: '',
      searchText: '',
      durum: '',
      createdDateStart: formatDate(oneMonthAgo),
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    this.getBankalar();
    this.getTedarikciler();
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

  filterChanged(createdDateStart: string, createdDateEnd: string) {
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

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

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getAllYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";

    return "";
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
