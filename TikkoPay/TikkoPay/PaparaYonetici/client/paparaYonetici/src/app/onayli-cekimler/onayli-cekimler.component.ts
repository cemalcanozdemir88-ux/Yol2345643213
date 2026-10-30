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
import { Durum } from '../_enums/durum.enum';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { NgForm } from '@angular/forms';
import { CekimCreateDto } from '../_models/yatirimTalepleri/cekimCreate.dto';
import { IslemTipi } from '../_enums/islemTipi.enum';

declare var $: any;

@Component({
  selector: 'app-onayli-cekimler',
  templateUrl: './onayli-cekimler.component.html',
  styleUrls: ['./onayli-cekimler.component.css']
})
export class OnayliCekimlerComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('manuelCekimEkleModalButton') manuelCekimEkleModalButton!: ElementRef;
  @ViewChild('manuelCekimEkleModalKapatButton') manuelCekimEkleModalKapatButton!: ElementRef;
  @ViewChild('yatirimSilModalKapatButton') yatirimSilModalKapatButton!: ElementRef;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];

  cekimCreateModel!: CekimCreateDto;
  isManuelCekimCreating = false;

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
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

      this.createEmptyCekimCreateModel();

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

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((response) => {
      this.webSiteleriDropDownModel = response.result;
    })
  }

  getToplamCekimToplami(){
    this.yatirimTalepleriService.getToplamCekimTutari(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.toplamKomisyon = response.result.toplamKomisyon;
    });
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getOnaylananCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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
    this.yatirimTalepleriService.exportToExcel(this.filterModel, IslemTip.Cekim, Durum.Onaylandi).subscribe((response) => {
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

  createEmptyCekimCreateModel() {
    this.cekimCreateModel = {
      tedarikciId: '',
      webSiteId: '',
      hesapNo: '',
      hesapSahibi: '',
      adSoyad: '',
      userName: '',
      userId: '',
      iptalAciklama: '',
      durum: Durum.Onaylandi,
      islemTipi: IslemTip.Cekim,
      tutar: 0,
    };
  }

  openManuelCekimEkleModal() {
    this.createEmptyCekimCreateModel();
    this.manuelCekimEkleModalButton.nativeElement.click();
  }

  closeManuelCekimEkleModal(){
    if (this.manuelCekimEkleModalKapatButton && this.manuelCekimEkleModalKapatButton.nativeElement) {
      this.manuelCekimEkleModalKapatButton.nativeElement.click();
    }

    this.createEmptyCekimCreateModel();
  }

  closeManuelCekimEkleModalKapat() {
    this.createEmptyCekimCreateModel();
  }

  submitManuelCekimEkleForm(manuelCekimEkleForm: NgForm) {
    if (!manuelCekimEkleForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isManuelCekimCreating = true;

    this.yatirimTalepleriService.createCekim(this.cekimCreateModel).subscribe({
      next: response => {
        this.isManuelCekimCreating = false;

        if (response.success) {
          this.closeManuelCekimEkleModal();
          if(this.isAdmin()){
            this.getToplamCekimToplami();
          }
          this.getAll();
          this.alertify.success("Kayıt eklendi");
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isManuelCekimCreating = false;
        this.alertify.error(error.error.message);
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
            this.getToplamCekimToplami();
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
          console.log(error);
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeYatirimSilModal();
        }
      });
    }
  }

}
