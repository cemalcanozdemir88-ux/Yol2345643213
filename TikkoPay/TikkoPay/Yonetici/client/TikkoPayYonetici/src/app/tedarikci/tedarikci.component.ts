import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { TedarikciService } from '../_services/tedarikci.service';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciCreateDto } from '../_models/tedarikci/tedarikciCreate.dto';
import { NgForm } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { Authorization } from '../_constants/authorizations';
import { RecordsPerPage } from '../_constants/constants';
import { TedarikcilerPagingFilter } from '../_models/tedarikci/tedarikcilerFilter';
import { WebSiteTedarikcileriService } from '../_services/websitetedarikcileri.service';
import { WebSiteTedarikcileriCreateDto } from '../_models/websitetedarikcileri/webSiteTedarikcileriCreate.dto';
import { WebSiteTedarikcileriListDto } from '../_models/websitetedarikcileri/webSiteTedarikcileriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';

@Component({
  selector: 'app-tedarikci',
  templateUrl: './tedarikci.component.html',
  styles: [
  ]
})
export class TedarikciComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('tedarikciEkleModalButton') tedarikciEkleModalButton!: ElementRef;
  @ViewChild('tedarikciEkleModalKapatButton') tedarikciEkleModalKapatButton!: ElementRef;
  @ViewChild('tedarikciSilModalKapatButton') tedarikciSilModalKapatButton!: ElementRef;
  @ViewChild('webSiteleriModalButton') webSiteleriModalButton!: ElementRef;
  @ViewChild('webSiteleriModalKapatButton') webSiteleriModalKapatButton!: ElementRef;

  tedarikciler: TedarikcilerListDto[] = [];
  webSiteTedarikcileri: WebSiteTedarikcileriListDto[] = [];
  webSiteleri: WebSiteleriListDto[] = [];
  tedarikciCreateModel!: TedarikciCreateDto;

  tedarikcilerFilter!: TedarikcilerPagingFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;
  public isStartingBakiyeJob: boolean = false;
  public isLoadingWebSiteleri: boolean = false;
  public isLoadingWebSiteOptions: boolean = false;
  public isAddingWebSite: boolean = false;
  public deletingWebSiteTedarikciId: string = '';

  selectedItemId: string = "";
  selectedTedarikciForWebSiteleri: string = '';
  selectedWebSiteToAdd: string = '';

  recordsPerPage: number = 40;
  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private tedarikciService: TedarikciService,
    private webSiteTedarikcileriService: WebSiteTedarikcileriService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

      this.tedarikcilerFilter = {
        bankaHavale: '',
        otoPuan: '',
        durum: '',
        pageNumber: 1,
        recordsPerPage: this.recordsPerPage
      }
  }

  ngOnInit(): void {
    this.createEmptyTedarikciCreateModel();
    this.getAll();
  }

  createEmptyTedarikciCreateModel() {
    this.tedarikciCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      unvan: '',
      bankaHavale: false,
      yaririmKomisyonOrani: 0,
      cekimKomisyonOrani: 0,
      tedarikcidenAlinanOdemeKomisyonOrani: 0,
      minYatirimTutari: 0,
      yatirimPuani: 0,
      hesapNoGosterimSayisi: 0,
      bakiyeLimitor: 0,
      otoPuan: false,
      durum: false
    };
  }

  filterChanged() {
    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.tedarikcilerFilter.pageNumber = pageNumber;

    this.tedarikciService.getAllWithPaging(this.tedarikcilerFilter).subscribe((response) => {
      if (response.success) {
        this.tedarikciler = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
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
      this.tedarikciService.delete(this.selectedItemId).subscribe({
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
          this.closeTedarikciSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeTedarikciSilModal();
        }
      });
    }
  }

  closeTedarikciSilModal() {
    if (this.tedarikciSilModalKapatButton && this.tedarikciSilModalKapatButton.nativeElement) {
      this.tedarikciSilModalKapatButton.nativeElement.click();
    }
  }

  startEdit(itemId: string) {
    this.tedarikciService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.tedarikciEkleModalButton && this.tedarikciEkleModalButton.nativeElement) {
            this.tedarikciCreateModel = response.result;
            this.tedarikciEkleModalButton.nativeElement.click();
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

  closeTedarikciEkleModal() {
    if (this.tedarikciEkleModalKapatButton && this.tedarikciEkleModalKapatButton.nativeElement) {
      this.tedarikciEkleModalKapatButton.nativeElement.click();
    }
  }

  openTedarikciEkleModal() {
    if (this.tedarikciEkleModalButton && this.tedarikciEkleModalButton.nativeElement) {
      this.createEmptyTedarikciCreateModel();
      this.tedarikciEkleModalButton.nativeElement.click();
    }
  }

  openWebSiteleriModal(tedarikciId: string) {
    this.selectedTedarikciForWebSiteleri = tedarikciId;
    this.selectedWebSiteToAdd = '';
    this.getWebSiteleri();
    this.loadWebSiteleri(tedarikciId);

    if (this.webSiteleriModalButton && this.webSiteleriModalButton.nativeElement) {
      this.webSiteleriModalButton.nativeElement.click();
    }
  }

  getWebSiteleri() {
    this.isLoadingWebSiteOptions = true;

    this.webSiteleriService.getSelectList().subscribe({
      next: (response) => {
        if (response.success) {
          this.webSiteleri = response.result;
        }
        else {
          this.alertify.error(response.message);
        }

        this.isLoadingWebSiteOptions = false;
      },
      error: () => {
        this.isLoadingWebSiteOptions = false;
        this.alertify.error('Web siteleri alınırken bir hata oluştu.');
      }
    });
  }

  addWebSiteToTedarikci() {
    if (!this.selectedWebSiteToAdd) {
      this.alertify.error('Bir web sitesi seçin.');
      return;
    }
    if (!this.selectedTedarikciForWebSiteleri) {
      this.alertify.error('Bir tedarikçi seçin.');
      return;
    }

    this.isAddingWebSite = true;

    const webSiteTedarikcileriCreateDto: WebSiteTedarikcileriCreateDto = {
      webSiteId: this.selectedWebSiteToAdd,
      tedarikciId: this.selectedTedarikciForWebSiteleri
    };

    this.webSiteTedarikcileriService.create(webSiteTedarikcileriCreateDto).subscribe({
      next: (response) => {
        if (response.success) {
          this.alertify.success('Web sitesi tedarikçiye eklendi');
          this.selectedWebSiteToAdd = '';
          this.loadWebSiteleri(this.selectedTedarikciForWebSiteleri);
        }
        else {
          this.alertify.error(response.message);
        }

        this.isAddingWebSite = false;
      },
      error: () => {
        this.isAddingWebSite = false;
        this.alertify.error('Web sitesi tedarikçiye eklenirken hata oluştu.');
      }
    });
  }

  removeWebSiteTedarikci(associationId: string) {
    if (!confirm('Web sitesi tedarikçisi silinecektir, devam etmek istiyor musunuz?')) {
      return;
    }

    this.deletingWebSiteTedarikciId = associationId;

    this.webSiteTedarikcileriService.delete(associationId).subscribe({
      next: (response) => {
        if (response.success) {
          this.alertify.success(response.message);
          this.loadWebSiteleri(this.selectedTedarikciForWebSiteleri);
        }
        else {
          this.alertify.error(response.message);
        }

        this.deletingWebSiteTedarikciId = '';
      },
      error: () => {
        this.alertify.error('Web sitesi tedarikçisi silinirken hata oluştu.');
        this.deletingWebSiteTedarikciId = '';
      }
    });
  }

  private loadWebSiteleri(tedarikciId: string) {
    this.webSiteTedarikcileri = [];
    this.isLoadingWebSiteleri = true;

    this.webSiteTedarikcileriService.getByTedarikciId(tedarikciId).subscribe({
      next: (response) => {
        this.isLoadingWebSiteleri = false;

        if (response.success) {
          this.webSiteTedarikcileri = response.result;
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoadingWebSiteleri = false;
        this.alertify.error('Web siteleri alınırken bir hata oluştu.');
      }
    });
  }

  closeWebSiteleriModal() {
    if (this.webSiteleriModalKapatButton && this.webSiteleriModalKapatButton.nativeElement) {
      this.webSiteleriModalKapatButton.nativeElement.click();
    }
  }

  startTedarikciBakiyeJob() {
    if (this.isStartingBakiyeJob) {
      return;
    }

    this.isStartingBakiyeJob = true;

    this.tedarikciService.startTedarikciBakiyeJob().subscribe({
      next: (response) => {
        const isSuccessful = response === true;

        if (isSuccessful) {
          this.alertify.success("Tedarikçi bakiyeleri güncellendi");
        }
        else {
          this.alertify.error('İşlem başlatılamadı');
        }
      },
      error: () => {
        this.isStartingBakiyeJob = false;
        this.alertify.error('İşlem başlatılırken bir hata oluştu');
      },
      complete: () => {
        this.isStartingBakiyeJob = false;
      }
    });
  }

  submitCreateTedarikciForm(tedarikciForm: NgForm) {

    if (!tedarikciForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    if (this.isCreate()) {
      this.tedarikciService.create(this.tedarikciCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeTedarikciEkleModal();
            this.createEmptyTedarikciCreateModel();
            this.getAll();
            this.alertify.success("Tedarikci eklendi");
          }
          else {
            this.alertify.error(response.message);
          }
          this.isCreating = false;
        },
        error: error => {
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }
    else {
      this.tedarikciService.update(this.tedarikciCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeTedarikciEkleModal();
            this.createEmptyTedarikciCreateModel();
            this.getAll();
            this.alertify.success("Tedarikci güncellendi");
          }
          else {
            this.alertify.error(response.message);
          }
          this.isCreating = false;
        },
        error: error => {
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }

  }

  closetedarikciEkleModalKapat() {
    this.createEmptyTedarikciCreateModel();
  }
  
  private isCreate(): boolean {
    return this.tedarikciCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }
}


