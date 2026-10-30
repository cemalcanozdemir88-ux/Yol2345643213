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
import { NgForm } from '@angular/forms';
import { Durum } from '../_enums/durum.enum';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { CekimCreateDto } from '../_models/cekimTalepleri/cekimCreate.dto';
import { MultiTedarikciYonlendirmeDto } from '../_models/cekimTalepleri/multi-tedarikci-yonlendirme.dto';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTaleperliList.dto';
import { CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { TedarikcilerSelectListWithBakiyeDto } from '../_models/tedarikci/tedarikcilerSelectListWithBakiye.dto';
import { ChangeDetectorRef } from '@angular/core';
import { CekimUpdateDto } from '../_models/cekimTalepleri/cekimUpdate.dto';

declare var $: any;

@Component({
  selector: 'app-bekleyen-cekimler',
  templateUrl: './bekleyen-cekimler.component.html',
  styleUrls: ['./bekleyen-cekimler.component.css']
})
export class BekleyenCekimlerComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('manuelCekimEkleModalButton') manuelCekimEkleModalButton!: ElementRef;
  @ViewChild('manuelCekimEkleModalKapatButton') manuelCekimEkleModalKapatButton!: ElementRef;
  @ViewChild('yatirimSilModalKapatButton') yatirimSilModalKapatButton!: ElementRef;
  @ViewChild('yonlendirModalKapatButton') yonlendirModalKapatButton!: ElementRef;
  @ViewChild('yonlendirModal') yonlendirModal!: ElementRef;
  @ViewChild('cekimGuncelleModal') cekimGuncelleModal!: ElementRef;
  @ViewChild('tedarikciSilModalKapatButton') tedarikciSilModalKapatButton!: ElementRef;

  public isForward: boolean = false;
  public isForwardLoading: boolean = false;
  public isApproveAndNotify: boolean = false;
  public isCancelAndNotify: boolean = false;
  public isCekimUpdating: boolean = false;

  cekimTalepleri: CekimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerSelectListWithBakiyeDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];
  cekimTedarikciAtamalari: YatirimTalepleriListDto[] = [];

  cekimCreateModel!: CekimCreateDto;
  isManuelCekimCreating = false;

  cekimUpdateModel!: CekimUpdateDto;

  filterModel!: CekimTalepleriPagingFilter;

  selectedItemId: string = "";
  selectedTedarikciId: string = "";

  public isDeleting: boolean = false;

  activePage: number = 1;
  totalPages: number = 0;

  totalAmount: number = 0;
  selectedTedarikciler: { id: string; amount: number }[] = [{ id: '', amount: 0 }];

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
    this.createEmptyCekimUpdateModel();

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      searchText: '',
      dekontKontrolDurum: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage,
      sortColumn: '1'
    }

  }

  ngOnInit(): void {

    if (this.isAdmin()) {
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
    this.tedarikcilerService.getSelectListWithBakiye().subscribe((reponse) => {
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

    this.cekimTalepleriService.getBekleyenCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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

  sort(column: string) {

    if (this.filterModel.sortColumn === column) {
      // aynı kolona tekrar tıklanırsa yönü değiştir
      this.filterModel.sortDirection = this.filterModel.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.filterModel.sortColumn = column;
      this.filterModel.sortDirection = 'asc';
    }

    this.getAll(); // yeni sıralama ile veriyi tekrar yükle
  }

  downloadExcel() {
    this.cekimTalepleriService.exportToExcel(this.filterModel, Durum.Bekliyor).subscribe((response) => {
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

    this.selectedItemId = item.id;

    this.cekimTalepleriService.getByTopId(item.id).subscribe({
      next: (response) => {
        if (response.success) {

          if (response.success) {
            this.cekimTedarikciAtamalari = response.result;
          } else {
            this.alertify.error(response.message);
          }

        } else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.alertify.error('Çekim tedarikçi atama bilgileri alınırken bir hata oluştu.');
      }
    });
  }

  startUpdate(item: CekimTalepleriListDto) {

    this.isForwardLoading = true;

    this.cekimTalepleriService.getById(item.id).subscribe({
      next: (response) => {
        if (response.success) {

          // if (response.result.length > 0) {
          //   this.selectedTedarikciler = response.result.map((atama: CekimTalepleriListDto) => ({
          //     id: atama.tedarikciId,
          //     amount: atama.tutar
          //   }));
          // } else {

          //   this.selectedTedarikciler = [{ id: '', amount: item.tutar }];
          // }

          this.cekimUpdateModel.id = response.result.id;
          this.cekimUpdateModel.adSoyad = response.result.adSoyad;
          this.cekimUpdateModel.userId = response.result.userId;
          this.cekimUpdateModel.userName = response.result.userName;

          this.isForwardLoading = false;

          this.cdr.detectChanges();
          $(this.cekimGuncelleModal.nativeElement).modal('show');

        } else {
          this.isForwardLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isForwardLoading = false;
        this.alertify.error('Tedarikçi bilgileri alınırken bir hata oluştu.');
      }
    });

  }

  submitCekimGuncelleForm(cekimGuncelleForm: NgForm) {

    if (!cekimGuncelleForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCekimUpdating = true;

    this.cekimTalepleriService.updateBekleyenCekim(this.cekimUpdateModel).subscribe({
      next: response => {
        this.isCekimUpdating = false;

        if (response.success) {
          this.closeCekimGuncelleModalKapat();

          this.getAll();
          this.alertify.success("Kayıt güncellendi");
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isCekimUpdating = false;
      },
      complete: () => {

      }
    });

  }

  closeCekimGuncelleModalKapat() {
    this.createEmptyCekimUpdateModel();
    $(this.cekimGuncelleModal.nativeElement).modal('hide');
  }

  startForward(item: CekimTalepleriListDto) {

    if (item.yonlendirilmeTarihi) {
      const isConfirmed = window.confirm("Çekim tekrar yönlendirilecektir. Devam etmek istiyor musunuz?");
      if (!isConfirmed) {
        return;
      }
    }

    this.isForwardLoading = true;

    this.totalAmount = item.tutar;
    this.selectedItemId = item.id;

    this.cekimTalepleriService.getByTopId(item.id).subscribe({
      next: (response) => {
        if (response.success) {

          if (response.result.length > 0) {
            this.selectedTedarikciler = response.result.map((atama: CekimTalepleriListDto) => ({
              id: atama.tedarikciId,
              amount: atama.tutar
            }));
          } else {

            this.selectedTedarikciler = [{ id: '', amount: item.tutar }];
          }

          this.isForwardLoading = false;

          this.cdr.detectChanges();
          $(this.yonlendirModal.nativeElement).modal('show');

        } else {
          this.isForwardLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isForwardLoading = false;
        this.alertify.error('Tedarikçi bilgileri alınırken bir hata oluştu.');
      }
    });
  }

  onaylaVeSiteyeBildir() {

    const isConfirmed = window.confirm("Çekim onaylanacaktır. Devam etmek istiyor musunuz?");
    if (!isConfirmed) {
      return;
    }

    if (!this.selectedItemId) {
      this.alertify.error("Çekim bulunamadı!");
      return;
    }

    this.isApproveAndNotify = true;

    this.cekimTalepleriService.onaylaVeSiteyeBildir(this.selectedItemId).subscribe({
      next: response => {
        this.isApproveAndNotify = false;

        if (response.success) {
          this.closeYonlendirModalKapat();
          this.getAll();
          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isApproveAndNotify = false;
      },
      complete: () => {
        this.isApproveAndNotify = false;
      }
    });
  }

  iptalEt() {

    const isConfirmed = window.confirm("Çekim iptal edilecektir. Devam etmek istiyor musunuz?");
    if (!isConfirmed) {
      return;
    }

    if (!this.selectedItemId) {
      this.alertify.error("Çekim bulunamadı!");
      return;
    }

    this.isCancelAndNotify = true;

    this.cekimTalepleriService.iptalEtVeSiteyeBildir(this.selectedItemId).subscribe({
      next: response => {
        this.isCancelAndNotify = false;

        if (response.success) {
          this.closeYonlendirModalKapat();
          this.getAll();
          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isCancelAndNotify = false;
      },
      complete: () => {
        this.isCancelAndNotify = false;
      }
    });
  }

  submitYonlendirForm(yonlendirForm: NgForm) {

    if (!yonlendirForm.valid) {
      this.alertify.error("Tedarkiçi seçiniz!");
      return;
    }

    const totalAssigned = this.selectedTedarikciler.reduce((sum, t) => sum + t.amount, 0);
    if (totalAssigned !== this.totalAmount) {
      this.alertify.error('Atanan tutar toplam tutar ile eşleşmiyor.');
      return;
    }

    const hasZeroAmount = this.selectedTedarikciler.some(t => t.amount <= 0);
    if (hasZeroAmount) {
      this.alertify.error('Tedarikçilere 0 veya negatif tutar atanamaz.');
      return;
    }

    const uniqueTedarikciIds = new Set(this.selectedTedarikciler.map(t => t.id));
    if (uniqueTedarikciIds.size !== this.selectedTedarikciler.length) {
      this.alertify.error('Aynı tedarikçi birden fazla seçilemez.');
      return;
    }

    const isAtesOtoHavaleSelected = this.selectedTedarikciler.some(t => t.id == 'c3b0aab6-d11e-41e5-bb3d-cfd18ed8f607');
    if (isAtesOtoHavaleSelected) {
      const confirmation = window.confirm("Ateş Oto havale seçildi. Bu tedarikçi otomatik onay vermektedir. İşlemi iptal edilemez, devam etmek istiyor musunuz?");
      if (!confirmation) {
        return;
      }
    }

    const request: MultiTedarikciYonlendirmeDto = {
      cekimTalepleriId: this.selectedItemId,
      tedarikciWithAmount: this.selectedTedarikciler.map(tedarikci => ({
        tedarikciId: tedarikci.id,
        amount: tedarikci.amount
      }))
    };

    this.isForward = true;

    this.cekimTalepleriService.multiTedarikciYonlendir(request).subscribe({
      next: response => {
        this.isForward = false;
        if (response.success) {
          this.closeYonlendirModalKapat();
          this.getAll();
          this.alertify.success(response.message);
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isForward = false;
      },
      complete: () => {
        this.isForward = false;
      }
    });
  }

  closeYonlendirModalKapat() {
    this.selectedItemId = "";
    this.selectedTedarikciId = "";
    this.totalAmount = 0;
    this.selectedTedarikciler = [{ id: '', amount: 0 }];
    this.yonlendirModalKapatButton.nativeElement.click();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }

  createEmptyCekimCreateModel() {
    this.cekimCreateModel = {
      tedarikciId: null,
      webSiteId: '',
      hesapNo: '',
      hesapSahibi: '',
      adSoyad: '',
      userName: '',
      userId: '',
      iptalAciklama: '',
      durum: Durum.Bekliyor,
      islemTipi: IslemTip.Cekim,
      tutar: 0,
    };
  }

  createEmptyCekimUpdateModel() {
    this.cekimUpdateModel = {
      id: '',
      adSoyad: '',
      userName: '',
      userId: ''
    }
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

  startTedarikciSil(itemId: string) {
    this.selectedItemId = itemId;
  }

  cancelTedarikciSil() {
    this.selectedItemId = "";
  }

  closeTedarikciSilModal() {
    if (this.tedarikciSilModalKapatButton && this.tedarikciSilModalKapatButton.nativeElement) {
      this.tedarikciSilModalKapatButton.nativeElement.click();
    }
  }

  cekimTalebindenTedarikciSil() {
    if (this.selectedItemId !== "") {
      this.isDeleting = true;
      this.cekimTalepleriService.tedarikciSil(this.selectedItemId).subscribe({
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

  addTedarikci() {
    this.selectedTedarikciler.push({ id: '', amount: 0 });
  }

  removeTedarikci(index: number) {
    this.selectedTedarikciler.splice(index, 1);
  }

}
