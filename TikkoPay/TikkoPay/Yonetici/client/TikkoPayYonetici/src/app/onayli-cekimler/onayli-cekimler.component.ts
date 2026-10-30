import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { AlertifyService } from '../_services/alertify.service';
import { BankaService } from '../_services/banka.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { Durum } from '../_enums/durum.enum';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { NgForm } from '@angular/forms';
import { CekimCreateDto } from '../_models/cekimTalepleri/cekimCreate.dto';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTaleperliList.dto';
import { CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { ChangeDetectorRef } from '@angular/core';
import { GetDekontDto } from '../_models/cekimTalepleri/getDekont.dto';
import { UpdateDekontDurumDto } from '../_models/cekimTalepleri/updateDekontDurum.dto';

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
  @ViewChild('cokluTedarikciModal') cokluTedarikciModal!: ElementRef;
  @ViewChild('dekontModal') dekontModal!: ElementRef;
  @ViewChild('dekontModalKapatButton') dekontModalKapatButton!: ElementRef;

  public isLoading: boolean = false;

  cekimTalepleri: CekimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];
  cekimTedarikciAtamalari: CekimTalepleriListDto[] = [];
  getDekontDto: GetDekontDto;

  cekimCreateModel!: CekimCreateDto;
  isManuelCekimCreating = false;
  isDekontDurumSubmit = false;

  filterModel!: CekimTalepleriPagingFilter;

  toplamTutar: number = 0;
  tedarikciToplamKomisyon: number = 0;
  websiteToplamKomisyon: number = 0;

  ortalamSureText: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  selectedItemId: string = "";
  public isDeleting: boolean = false;

  private cdr: ChangeDetectorRef;

  constructor(
    cdr: ChangeDetectorRef,
    private authService: AuthService,
    private cekimTalepleriService: CekimTalepleriService,
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

    this.cdr = cdr;
    this.createEmptyCekimCreateModel();

    const today = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1);

    const formatDate = (date: Date) => {
      return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
    }

    this.getDekontDto = {
      id: '',
      dekontKontrolDurumu: 0,
      dekontKontrolKullanici: '',
      dekontKontrolTarihi: '',
      info: ''
    }

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      searchText: '',
      dekontKontrolDurum: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: formatDate(oneMonthAgo),
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: 40
    }


  }

  ngOnInit(): void {
    if (this.isAdmin()) {
      this.getBankalar();
      this.getTedarikciler();
      this.getToplamCekimToplami();
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
      this.getToplamCekimToplami();
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

  getToplamCekimToplami() {
    this.cekimTalepleriService.getToplamCekimTutari(this.filterModel).subscribe((response) => {
      this.toplamTutar = response.result.toplamTutar;
      this.tedarikciToplamKomisyon = response.result.tedarikciToplamKomisyon;
      this.websiteToplamKomisyon = response.result.websiteToplamKomisyon;
    });
  }

  getOrtalamaOnaySuresi() {
    this.cekimTalepleriService.getOrtalamaOnaySuresi(this.filterModel).subscribe((response) => {
      this.ortalamSureText = response.result.formatliSure;
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
    this.cekimTalepleriService.exportToExcel(this.filterModel, Durum.Onaylandi).subscribe((response) => {
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

  startCokluTedarikciModal(item: CekimTalepleriListDto) {

    this.isLoading = true;

    this.cekimTalepleriService.getByTopId(item.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.cekimTedarikciAtamalari = response.result;

          this.isLoading = false;

          this.cdr.detectChanges();
          $(this.cokluTedarikciModal.nativeElement).modal('show');

        } else {
          this.isLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.alertify.error('Çekim tedarikçi atama bilgileri alınırken bir hata oluştu.');
      }
    });
  }

  startDekontModal(item: CekimTalepleriListDto) {
    this.isLoading = true;

    this.getDekontDto = {
      id: '',
      dekontKontrolDurumu: 0,
      dekontKontrolKullanici: '',
      dekontKontrolTarihi: '',
      info: ''
    }

    this.cekimTalepleriService.getDekont(item.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.getDekontDto = response.result;

          this.isLoading = false;

          this.cdr.detectChanges();
          $(this.dekontModal.nativeElement).modal('show');

        } else {
          this.isLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.alertify.error('Dekont bilgileri alınırken bir hata oluştu.');
      }
    });

  }

  toplamCekimTutar(): number {
    return this.cekimTalepleri.reduce((sum, item) => sum + item.tutar, 0);
  }
  toplamKomisyonTutar(): number {
    return this.cekimTalepleri.reduce((sum, item) => sum + item.tedarikciKomisyon, 0);
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

  closeManuelCekimEkleModal() {
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

    this.cekimTalepleriService.createCekim(this.cekimCreateModel).subscribe({
      next: response => {
        this.isManuelCekimCreating = false;

        if (response.success) {
          this.closeManuelCekimEkleModal();
          if (this.isAdmin()) {
            this.getToplamCekimToplami();
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
        this.isManuelCekimCreating = false;
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

  deleteRecord() {
    if (this.selectedItemId !== "") {
      this.isDeleting = true;
      this.cekimTalepleriService.delete(this.selectedItemId).subscribe({
        next: (response) => {
          if (this.isAdmin()) {
            this.getToplamCekimToplami();
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

    closeDekontModal() {
    if (this.dekontModalKapatButton && this.dekontModalKapatButton.nativeElement) {
      this.dekontModalKapatButton.nativeElement.click();
    }
  }

  submitDekontKontrolDurumu(dekontKontrolDurumForm: NgForm) {

    // const isConfirmed = window.confirm("Dekont kontrol durumu değiştirilecektir. Devam etmek istiyor musunuz?");
    // if (!isConfirmed) {
    //   return; 
    // }

    this.isDekontDurumSubmit = true;

    let updateDekontDurumDto: UpdateDekontDurumDto = {
      id: this.getDekontDto.id,
      dekontKontrolDurumu: this.getDekontDto.dekontKontrolDurumu
    }

    this.cekimTalepleriService.updateDekontDurum(updateDekontDurumDto).subscribe({
      next: response => {
        if (response.success) {
          this.closeDekontModal();
          this.getAll();
          this.alertify.success("Kayıt güncellendi");
        }
        else {
          this.alertify.error(response.message);
        }
        this.isDekontDurumSubmit = false;
      },
      error: error => {
        this.isDekontDurumSubmit = false;
      },
      complete: () => {
        this.isDekontDurumSubmit = false;
      }
    });

  }

  formatDateToSlash(date: Date): string {
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\./g, '/');
  }

}
