import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Durum } from '../_enums/durum.enum';
import { WebSiteKasaHaraketleriCreateDto } from '../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriCreate.dto';
import { WebSiteKasaHaraketleriFilter, WebSiteKasaHaraketleriPagingFilter } from '../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriFilter';
import { WebSiteKasaHaraketleriListDto } from '../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriList.dto';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { AlertifyService } from '../_services/alertify.service';
import { WebSiteKasaHaraketleriService } from '../_services/websiteKasaHaraketleri.service';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { Authorization } from '../_constants/authorizations';
import { AuthService } from '../_services/auth.service';
import { RecordsPerPage } from '../_constants/constants';

declare var $: any;

@Component({
  selector: 'app-kasa-haraketleri-site',
  templateUrl: './kasa-haraketleri-site.component.html',
  styleUrls: ['./kasa-haraketleri-site.component.css']
})
export class KasaHaraketleriSiteComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kasaHaraketleriEkleModalButton') kasaHaraketleriEkleModalButton!: ElementRef;
  @ViewChild('kasaHaraketleriEkleModalKapatButton') kasaHaraketleriEkleModalKapatButton!: ElementRef;
  @ViewChild('kasaHaraketleriSilModalKapatButton') kasaHaraketleriSilModalKapatButton!: ElementRef;

  webSiteleriDronDownModel: WebSiteleriListDto[] = [];

  kasaHaraketleriList: WebSiteKasaHaraketleriListDto[] = [];
  kasaHaraketleriCreateModel!: WebSiteKasaHaraketleriCreateDto;

  kasaHaraketleriFilter!: WebSiteKasaHaraketleriPagingFilter;

  kasaGirisCikisToplamTitle: string = "Websitelerinden Alınan Toplam Ödeme";
  kasaGirisGirisToplamTitle: string = "Websitelerine Yapılan Toplam Ödeme";


  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private authService: AuthService,
    private kasaHaraketleriService: WebSiteKasaHaraketleriService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {
    this.createEmptyModel();

    this.kasaHaraketleriFilter = {
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      durum: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {

    if(this.isWebSiteKullanicisi()){
      this.kasaGirisCikisToplamTitle = "Yapılan Ödeme";
      this.kasaGirisGirisToplamTitle = "Alınan Ödeme";
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

  createEmptyModel() {
    this.kasaHaraketleriCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      webSitesiId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      aciklama: '',
      tutar: 0,
      dovizKuru: 0,
      usdtMiktar: 0,
      usdtAdres: '',
      usdtTxtNo: '',
      resimUrl: '',
      islemTipi: this.isWebSiteKullanicisi() ? 2: 1,
      durum: 1
    };
  }

  filterChanged(createdDateStart: string, createdDateEnd: string) {
    this.kasaHaraketleriFilter.createdDateStart = createdDateStart;
    this.kasaHaraketleriFilter.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((reponse) => {
      this.webSiteleriDronDownModel = reponse.result;
    });
  }

  getAll(pageNumber: number = 1) {

    this.kasaHaraketleriFilter.pageNumber = pageNumber;

    this.kasaHaraketleriService.getAllWithPaging(this.kasaHaraketleriFilter).subscribe((response) => {
      if (response.success) {
        this.kasaHaraketleriList = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  downloadExcel() {
    this.kasaHaraketleriService.exportToExcel(this.kasaHaraketleriFilter).subscribe((response) => {
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

  closeKasaHaraketleriSilModal() {
    if (this.kasaHaraketleriSilModalKapatButton && this.kasaHaraketleriSilModalKapatButton.nativeElement) {
      this.kasaHaraketleriSilModalKapatButton.nativeElement.click();
    }
  }

  cancelDelete() {
    this.selectedItemId = "";
  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
  }

  deleteRecord() {
    if (this.selectedItemId !== "") {
      this.isDeleting = true;
      this.kasaHaraketleriService.delete(this.selectedItemId).subscribe({
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
          console.log(error);
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeKasaHaraketleriSilModal();
        }
      });
    }
  }

  startEdit(itemId: string) {
    this.kasaHaraketleriService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.kasaHaraketleriEkleModalButton && this.kasaHaraketleriEkleModalButton.nativeElement) {
            this.kasaHaraketleriCreateModel = {
              id: response.result.id,
              webSitesiId: response.result.webSitesiId,
              aciklama: response.result.aciklama,
              tutar: response.result.tutar,
              dovizKuru: response.result.dovizKuru,
              usdtMiktar: response.result.usdtMiktar,
              usdtAdres: response.result.usdtAdres,
              usdtTxtNo: response.result.usdtTxtNo,
              resimUrl: response.result.resimUrl,
              islemTipi: response.result.islemTipi,
              durum: response.result.durum
            };

            this.kasaHaraketleriEkleModalButton.nativeElement.click();
          }
        }
      },
      error: error => {
        console.log(error);
      },
      complete: () => {
      }
    })
  }

  closeKasaHaraketleriEkleModal() {
    if (this.kasaHaraketleriEkleModalKapatButton && this.kasaHaraketleriEkleModalKapatButton.nativeElement) {
      this.kasaHaraketleriEkleModalKapatButton.nativeElement.click();
    }
  }

  openKasaHaraketleriEkleModal() {
    this.createEmptyModel();
    this.kasaHaraketleriEkleModalButton.nativeElement.click();
  }

  submitCreateKasaHaraketleriForm(kasaHaraketleriForm: NgForm) {
    if (!kasaHaraketleriForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.kasaHaraketleriCreateModel.durum = Number(this.kasaHaraketleriCreateModel.durum);

    this.isCreating = true;

    if (this.isCreate()) {
      this.kasaHaraketleriService.create(this.kasaHaraketleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKasaHaraketleriEkleModal();
            this.createEmptyModel();
            this.getAll();
            this.alertify.success("Kayıt eklendi");
          }
          else {
            this.alertify.error(response.message);
          }
          this.isCreating = false;
        },
        error: error => {
          this.alertify.error(error.error.message);
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }
    else {
      this.kasaHaraketleriService.update(this.kasaHaraketleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKasaHaraketleriEkleModal();
            this.createEmptyModel();
            this.getAll();
            this.alertify.success("Kayıt güncellendi");
          }
          else {
            this.alertify.error(response.message);
          }
          this.isCreating = false;
        },
        error: error => {
          this.alertify.error(error.error.message);
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }

  }

  closeKasaHaraketleriEkleModalKapat() {
    this.createEmptyModel();
  }

  private isCreate(): boolean {
    return this.kasaHaraketleriCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

  getDurumText(durum: number) {
    if (durum == Durum.Bekliyor) return "Bekliyor";
    if (durum == Durum.Onaylandi) return "Onaylandı";
    if (durum == Durum.Red) return "Red";

    return "";
  }

  kasaGirisCikisToplam(islemTipi: number): number {
    return this.kasaHaraketleriList.filter(item => item.islemTipi === islemTipi).reduce((sum, item) => sum + item.tutar, 0)
  }

  getAraToplam(){
    if(this.isWebSiteKullanicisi()){
      return this.kasaGirisCikisToplam(2) - this.kasaGirisCikisToplam(1);
    }

    return this.kasaGirisCikisToplam(1) - this.kasaGirisCikisToplam(2);
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }
}
