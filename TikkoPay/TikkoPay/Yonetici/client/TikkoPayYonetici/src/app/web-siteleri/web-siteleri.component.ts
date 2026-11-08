import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { WebSiteleriCreateDto } from '../_models/websiteleri/websiteleriCreate.dto';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { NgForm } from '@angular/forms';
import { Authorization } from '../_constants/authorizations';
import { AffiliateService } from '../_services/affiliate.service';
import { AffiliateDto } from '../_models/affiliate/affiliate.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { WebSiteTedarikcileriService } from '../_services/websitetedarikcileri.service';
import { WebSiteTedarikcileriCreateDto } from '../_models/websitetedarikcileri/webSiteTedarikcileriCreate.dto';
import { WebSiteTedarikcileriListDto } from '../_models/websitetedarikcileri/webSiteTedarikcileriList.dto';
import { WebSiteTedarikcileriManualService } from '../_services/websitetedarikcileriManual.service';
import { WebSiteTedarikcileriManualCreateDto } from '../_models/websitetedarikcileriManual/webSiteTedarikcileriManualCreate.dto';
import { WebSiteTedarikcileriManualListDto } from '../_models/websitetedarikcileriManual/webSiteTedarikcileriManualList.dto';
import { PaymentMethodsDto } from '../_models/paymentMethods/paymentMethods.dto';
import { PaymentMethodsService } from '../_services/paymentMethods.service';
import { WebSiteHavaleBankalariService } from '../_services/websitehavalebankalari.service';
import { WebSiteHavaleBankalariListDto } from '../_models/websitehavalebankalari/webSiteHavaleBankalari.dto';
import { HavaleBankalariService } from '../_services/havalebankalari.service';
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';

@Component({
  selector: 'app-web-siteleri',
  templateUrl: './web-siteleri.component.html',
  styleUrls: ['./web-siteleri.component.css']
})
export class WebSiteleriComponent implements OnInit {
  Authorization = Authorization;

  @ViewChild('webSiteEkleModalButton') webSiteEkleModalButton!: ElementRef;
  @ViewChild('webSiteEkleModalKapatButton') webSiteEkleModalKapatButton!: ElementRef;
  @ViewChild('webSiteSilModalKapatButton') webSiteSilModalKapatButton!: ElementRef;

  @ViewChild('tedarikciModalButton') tedarikciModalButton!: ElementRef;
  @ViewChild('tedarikciModalKapatButton') tedarikciModalKapatButton!: ElementRef;

  @ViewChild('tedarikciManualModalButton') tedarikciManualModalButton!: ElementRef;
  @ViewChild('tedarikciManualModalKapatButton') tedarikciManualModalKapatButton!: ElementRef;

  @ViewChild('hesapModalButton') hesapModalButton!: ElementRef;

  // Web sitesine bağlı banka havale hesapları
  webSiteHesaplari: WebSiteHavaleBankalariListDto[] = [];
  hesapTedarikciId: string = '';
  tedarikciHesaplari: HavaleBankalariListDto[] = [];
  selectedHesapToAdd: string = '';
  isLoadingHesaplar: boolean = false;
  isAddingHesap: boolean = false;
  deletingHesapId: string = '';

  webSiteleri: WebSiteleriListDto[] = [];
  webSiteleriCreateModel!: WebSiteleriCreateDto;
  affiliateDronDownModel: AffiliateDto[] = [];
  paymentMethods: PaymentMethodsDto[] = [];

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteTedarikcileri: WebSiteTedarikcileriListDto[] = [];
  webSiteTedarikcileriManual: WebSiteTedarikcileriManualListDto[] = [];
  selectedTedarikciToAdd: string = '';

  public isDeleting: boolean = false;
  public isCreating: boolean = false;
  public isAddingTedarikci: boolean = false;
  public isLoadingWebSiteTedarikcileri: boolean = false;

  public deletingTedarikciId: string = '';

  selectedItemId: string = "";

  constructor(
    private affiliateService: AffiliateService,
    private webSiteleriService: WebSiteleriService,
    private tedarikcilerService: TedarikciService,
    private webSiteTedarikciService: WebSiteTedarikcileriService,
    private webSiteTedarikciManualService: WebSiteTedarikcileriManualService,
    private paymentMethodsSerivce: PaymentMethodsService,
    private webSiteHavaleBankalariService: WebSiteHavaleBankalariService,
    private havaleBankalariService: HavaleBankalariService,
    private alertify: AlertifyService) {

  }

  ngOnInit(): void {
    this.createEmptyWebSiteleriCreateModel();
    this.getAll();
    this.getAffiliates();
    this.getPaymentMethods();
  }

  createEmptyWebSiteleriCreateModel() {
    this.webSiteleriCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      webSiteUrl: '',
      telegramChannelId: '',
      baslik: '',
      kisaBaslik: '',
      affiliateId: null,
      affiliateYaririmKomisyonOrani: null,
      affiliateCekimKomisyonOrani: null,
      yaririmKomisyonOrani: 0,
      cekimKomisyonOrani: 0,
      odemeKomisyonOrani: 0,
      apiClientId: '',
      apiClientSecret: '',
      depositCallbackUrl: '',
      withdrawCallbackUrl: '',
      type: 1,
      paymentMethodId: null,
      bakiyeLimitor: 0,
      minYatirimTutari: 1000,
      minCekimTutari: 1000
    };
  }

  getAffiliates() {
    this.affiliateService.getAll().subscribe((reponse) => {
      this.affiliateDronDownModel = reponse.result;
    });
  }

  getPaymentMethods() {
    this.paymentMethodsSerivce.getAll().subscribe({
      next: (response) => {
        if (response.success) {
          this.paymentMethods = response.result ?? [];
        } else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.paymentMethods = [];
      }
    });
  }

  onPaymentMethodChange() {
    const selectedPaymentMethod = this.paymentMethods.find(x => x.id === this.webSiteleriCreateModel.paymentMethodId);

    if (selectedPaymentMethod) {
      this.webSiteleriCreateModel.depositCallbackUrl = selectedPaymentMethod.depositCallbackUrl ?? '';
      this.webSiteleriCreateModel.withdrawCallbackUrl = selectedPaymentMethod.withdrawCallbackUrl ?? '';
    } else {
      this.webSiteleriCreateModel.depositCallbackUrl = '';
      this.webSiteleriCreateModel.withdrawCallbackUrl = '';
    }
  }

  getAll() {
    this.webSiteleriService.getAll().subscribe((response) => {
      if (response.success) {
        this.webSiteleri = response.result;
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
      this.webSiteleriService.delete(this.selectedItemId).subscribe({
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
          this.closeWebSiteleriSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeWebSiteleriSilModal();
        }
      });
    }
  }

  closeWebSiteleriSilModal() {
    if (this.webSiteSilModalKapatButton && this.webSiteSilModalKapatButton.nativeElement) {
      this.webSiteSilModalKapatButton.nativeElement.click();
    }
  }

  startEdit(itemId: string) {
    this.webSiteleriService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.webSiteEkleModalButton && this.webSiteEkleModalButton.nativeElement) {
            this.webSiteleriCreateModel = response.result;
            this.webSiteEkleModalButton.nativeElement.click();
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

  closeWebSitesiEkleModal() {
    if (this.webSiteEkleModalKapatButton && this.webSiteEkleModalKapatButton.nativeElement) {
      this.webSiteEkleModalKapatButton.nativeElement.click();
    }
  }

  openWebSiteEkleModal() {
    if (this.webSiteEkleModalButton && this.webSiteEkleModalButton.nativeElement) {
      this.createEmptyWebSiteleriCreateModel();
      this.webSiteEkleModalButton.nativeElement.click();
    }
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getWebSiteTedarikcileriManual(webSiteId: string) {
    this.webSiteTedarikciManualService.getByWebSiteId(webSiteId).subscribe((response) => {
      if (response.success) {
        this.webSiteTedarikcileriManual = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  getWebSiteTedarikcileri(webSiteId: string) {
    this.isLoadingWebSiteTedarikcileri = true;

    this.webSiteTedarikciService.getByWebSiteId(webSiteId).subscribe({
      next: (response) => {
        this.isLoadingWebSiteTedarikcileri = false;

        if (response.success) {
          this.webSiteTedarikcileri = response.result;
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoadingWebSiteTedarikcileri = false;
        this.alertify.error('Tedarikçiler alınırken bir hata oluştu.');
      }
    });
  }

  openTedarikciManualModal(itemId: string) {

    this.selectedItemId = itemId;
    this.selectedTedarikciToAdd = '';
    this.webSiteTedarikcileri = [];

    this.getTedarikciler();
    this.getWebSiteTedarikcileriManual(itemId);

    if (this.tedarikciManualModalButton && this.tedarikciManualModalButton.nativeElement) {
      this.tedarikciManualModalButton.nativeElement.click();
    }
  }

  closeTedarikciManualModal() {
    if (this.tedarikciManualModalKapatButton && this.tedarikciManualModalKapatButton.nativeElement) {
      this.tedarikciManualModalKapatButton.nativeElement.click();
    }
  }

  addTedarikciToWebSiteManual() {
    if (!this.selectedTedarikciToAdd) {
      this.alertify.error('Bir tedarikçi seçin.');
      return;
    }
    if (!this.selectedItemId) {
      this.alertify.error('Bir website seçin.');
      return;
    }
    this.isAddingTedarikci = true;

    let webSiteTedarikcileriManualCreateDto: WebSiteTedarikcileriManualCreateDto = {
      webSiteId: this.selectedItemId,
      tedarikciId: this.selectedTedarikciToAdd
    };

    this.webSiteTedarikciManualService.create(webSiteTedarikcileriManualCreateDto).subscribe({
      next: response => {
        if (response.success) {

          this.alertify.success("Web sitesi tedarikçisi eklendi");
          this.getWebSiteTedarikcileriManual(this.selectedItemId);
        }
        else {
          this.alertify.error(response.message);
        }
        this.isAddingTedarikci = false;
      },
      error: error => {
        this.isAddingTedarikci = false;
      },
      complete: () => {
        this.isAddingTedarikci = false;
      }
    });
  }

  removeTedarikciFromWebSiteManual(associationId: string) {
    if (!confirm('Tedarikçi silinecektir, devam etmek istiyor musunuz?')) {
      return;
    }

    this.deletingTedarikciId = associationId;

    this.webSiteTedarikciManualService.delete(associationId).subscribe({
      next: (response) => {
        this.getWebSiteTedarikcileriManual(this.selectedItemId);

        if (response.success) {
          this.alertify.success(response.message);
        } else {
          this.alertify.error(response.message);
        }

        this.deletingTedarikciId = '';
      },
      error: error => {
        this.alertify.error('Tedarikçi silinirken hata oluştu.');
        this.deletingTedarikciId = '';
      },
      complete: () => {
      }
    });
  }


  openTedarikciModal(itemId: string) {

    this.selectedItemId = itemId;
    this.selectedTedarikciToAdd = '';
    this.webSiteTedarikcileri = [];

    this.getTedarikciler();
    this.getWebSiteTedarikcileri(itemId);

    if (this.tedarikciModalButton && this.tedarikciModalButton.nativeElement) {
      this.tedarikciModalButton.nativeElement.click();
    }
  }

  closeTedarikciModal() {
    if (this.tedarikciModalKapatButton && this.tedarikciModalKapatButton.nativeElement) {
      this.tedarikciModalKapatButton.nativeElement.click();
    }
  }

  addTedarikciToWebSite() {
    if (!this.selectedTedarikciToAdd) {
      this.alertify.error('Bir tedarikçi seçin.');
      return;
    }
    if (!this.selectedItemId) {
      this.alertify.error('Bir website seçin.');
      return;
    }
    this.isAddingTedarikci = true;

    let webSiteTedarikcileriCreateDto: WebSiteTedarikcileriCreateDto = {
      webSiteId: this.selectedItemId,
      tedarikciId: this.selectedTedarikciToAdd
    };

    this.webSiteTedarikciService.create(webSiteTedarikcileriCreateDto).subscribe({
      next: response => {
        if (response.success) {

          this.alertify.success("Web sitesi tedarikçisi eklendi");
          this.getWebSiteTedarikcileri(this.selectedItemId);
        }
        else {
          this.alertify.error(response.message);
        }
        this.isAddingTedarikci = false;
      },
      error: error => {
        this.isAddingTedarikci = false;
      },
      complete: () => {
        this.isAddingTedarikci = false;
      }
    });
  }

  removeTedarikciFromWebSite(associationId: string) {
    if (!confirm('Tedarikçi silinecektir, devam etmek istiyor musunuz?')) {
      return;
    }

    this.deletingTedarikciId = associationId;

    this.webSiteTedarikciService.delete(associationId).subscribe({
      next: (response) => {
        this.getWebSiteTedarikcileri(this.selectedItemId);

        if (response.success) {
          this.alertify.success(response.message);
        } else {
          this.alertify.error(response.message);
        }

        this.deletingTedarikciId = '';
      },
      error: error => {
        this.alertify.error('Tedarikçi silinirken hata oluştu.');
        this.deletingTedarikciId = '';
      },
      complete: () => {
      }
    });
  }

  // ---------------- Web sitesi banka havale hesapları ----------------

  openHesapModal(itemId: string) {
    this.selectedItemId = itemId;
    this.hesapTedarikciId = '';
    this.tedarikciHesaplari = [];
    this.selectedHesapToAdd = '';
    this.webSiteHesaplari = [];

    this.getTedarikciler();
    this.getWebSiteHesaplari(itemId);

    if (this.hesapModalButton && this.hesapModalButton.nativeElement) {
      this.hesapModalButton.nativeElement.click();
    }
  }

  getWebSiteHesaplari(webSiteId: string) {
    this.isLoadingHesaplar = true;

    this.webSiteHavaleBankalariService.getByWebSiteId(webSiteId).subscribe({
      next: (response) => {
        this.isLoadingHesaplar = false;
        if (response.success) {
          this.webSiteHesaplari = response.result ?? [];
        } else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoadingHesaplar = false;
        this.alertify.error('Hesaplar alınırken bir hata oluştu.');
      }
    });
  }

  onHesapTedarikciChange() {
    this.selectedHesapToAdd = '';
    this.tedarikciHesaplari = [];

    if (!this.hesapTedarikciId) {
      return;
    }

    this.havaleBankalariService.getByTedarikciId(this.hesapTedarikciId).subscribe({
      next: (response) => {
        if (response.success) {
          this.tedarikciHesaplari = response.result ?? [];
        } else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.alertify.error('Tedarikçinin hesapları alınırken bir hata oluştu.');
      }
    });
  }

  isHesapBagli(havaleBankaId: string): boolean {
    return this.webSiteHesaplari.some(x => x.havaleBankaId === havaleBankaId);
  }

  addHesapToWebSite() {
    if (!this.selectedHesapToAdd) {
      this.alertify.error('Bir hesap seçin.');
      return;
    }
    if (!this.selectedItemId) {
      this.alertify.error('Bir website seçin.');
      return;
    }

    this.isAddingHesap = true;

    this.webSiteHavaleBankalariService.create({
      webSiteId: this.selectedItemId,
      havaleBankaId: this.selectedHesapToAdd
    }).subscribe({
      next: response => {
        this.isAddingHesap = false;
        if (response.success) {
          this.alertify.success('Hesap web sitesine bağlandı');
          this.selectedHesapToAdd = '';
          this.getWebSiteHesaplari(this.selectedItemId);
        } else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isAddingHesap = false;
        this.alertify.error(error?.error?.message ?? 'Hesap bağlanırken hata oluştu.');
      }
    });
  }

  removeHesapFromWebSite(id: string) {
    if (!confirm('Hesap bağlantısı silinecektir, devam etmek istiyor musunuz?')) {
      return;
    }

    this.deletingHesapId = id;

    this.webSiteHavaleBankalariService.delete(id).subscribe({
      next: (response) => {
        this.deletingHesapId = '';
        this.getWebSiteHesaplari(this.selectedItemId);

        if (response.success) {
          this.alertify.success(response.message);
        } else {
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.deletingHesapId = '';
        this.alertify.error('Hesap bağlantısı silinirken hata oluştu.');
      }
    });
  }

  submitCreateWebSiteForm(webSiteForm: NgForm) {

    if (!webSiteForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    if (this.isCreate()) {
      this.webSiteleriService.create(this.webSiteleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeWebSitesiEkleModal();
            this.createEmptyWebSiteleriCreateModel();
            this.getAll();
            this.alertify.success("Web sitesi eklendi");
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
      this.webSiteleriService.update(this.webSiteleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeWebSitesiEkleModal();
            this.createEmptyWebSiteleriCreateModel();
            this.getAll();
            this.alertify.success("Web sitesi güncellendi");
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

  closeWebSiteEkleModalKapat() {
    this.createEmptyWebSiteleriCreateModel();
  }

  generateApiClientId() {
    const length = 32;
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let clientId = '';
    for (let i = 0; i < length; i++) {
      clientId += characters.charAt(Math.floor(Math.random() * characters.length));
    }

    this.webSiteleriCreateModel.apiClientId = clientId;
  }

  generateApiSecretId() {
    const length = 64;
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let secretId = '';
    for (let i = 0; i < length; i++) {
      secretId += characters.charAt(Math.floor(Math.random() * characters.length));
    }

    this.webSiteleriCreateModel.apiClientSecret = secretId;
  }


  public isCreate(): boolean {
    return this.webSiteleriCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

}
