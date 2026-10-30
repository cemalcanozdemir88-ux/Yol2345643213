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
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { Durum } from '../_enums/durum.enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { AuthService } from '../_services/auth.service';

declare var $: any;

@Component({
  selector: 'app-bekleyen-yatirimlar',
  templateUrl: './bekleyen-yatirimlar.component.html',
  styleUrls: ['./bekleyen-yatirimlar.component.css']
})
export class BekleyenYatirimlarComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('updatePriceModalButton') updatePriceModalButton!: ElementRef;
  @ViewChild('updatePriceModalKapatButton') updatePriceModalKapatButton!: ElementRef;

  @ViewChild('iptalModalKapatButton') iptalModalKapatButton!: ElementRef;
  @ViewChild('onayModalKapatButton') onayModalKapatButton!: ElementRef;


  updatePriceModel!: YatirimTalepleriListDto;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  public isCancelling: boolean = false;
  public isApproving: boolean = false;

  selectedItemId: string = '';
  iptalAciklama: string = '';

  activePage: number = 1;
  totalPages: number = 0;

  public isUpdating: boolean = false;

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

    this.updatePriceModel = new YatirimTalepleriListDto();

   
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

  filterChanged(createdDateStart: string, createdDateEnd: string) {
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getBekleyenYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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

  startEditPrice(itemId: string) {
    this.yatirimTalepleriService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.updatePriceModalButton && this.updatePriceModalButton.nativeElement) {
            this.updatePriceModel = response.result;
            this.updatePriceModalButton.nativeElement.click();
          }
        }
      },
      error: error => {
        alert(error);
      },
      complete: () => {

      }
    })
  }

  submitUpdatePriceForm() {

    this.isUpdating = true;

    this.yatirimTalepleriService.updatePrice(this.updatePriceModel.id, this.updatePriceModel.tutar, this.updatePriceModel.adSoyadYatirimYapan).subscribe({
      next: response => {
        if (response.success) {
          this.closeUpdatePriceModalKapat();
          this.getAll();

          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }

        this.isUpdating = false;

      },
      error: error => {
        this.isUpdating = false;
      },
      complete: () => {
        this.isUpdating = false;
      }
    });
  }

  closeUpdatePriceModalKapat() {
    this.updatePriceModalKapatButton.nativeElement.click();
    this.updatePriceModel = new YatirimTalepleriListDto();
  }


  closeCancel() {
    this.selectedItemId = '';
  }

    startCancel(itemId: string) {
    this.selectedItemId = itemId;
  }

    closeIptalModal() {
    if (
      this.iptalModalKapatButton &&
      this.iptalModalKapatButton.nativeElement
    ) {
      this.iptalModalKapatButton.nativeElement.click();
    }
  }

    cancelRecord() {

    if(this.iptalAciklama == ''){
      alert("Açıklama giriniz");
      return;
    }

    if (this.selectedItemId !== '') {
      this.isCancelling = true;

      this.yatirimTalepleriService.cancelItem(this.selectedItemId, this.iptalAciklama).subscribe({
        next: (response) => {
          
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }

          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        error: (error) => {
          console.log(error);

          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        complete: () => {
          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
      });
    }
    else {
      this.isCancelling = false;
      this.closeIptalModal();
      this.alertify.error("İptal edilecek kayıt seçilmedi");
    }
  }

  closeApprove() {
    this.selectedItemId = '';
  }

  startApprove(itemId: string) {
    this.selectedItemId = itemId;
  }

  closeApproveModal() {
    if (this.onayModalKapatButton && this.onayModalKapatButton.nativeElement) {
      this.onayModalKapatButton.nativeElement.click();
    }
  }

  approveRecord() {
    if (this.selectedItemId !== '') {
      this.isApproving = true;
      this.yatirimTalepleriService.approveItem(this.selectedItemId).subscribe({
        next: (response) => {
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }
        },
        error: (error) => {
          console.log(error);
        },
        complete: () => {
          this.selectedItemId = '';
          this.isApproving = false;
          this.closeApproveModal();
        },
      });
    }
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
