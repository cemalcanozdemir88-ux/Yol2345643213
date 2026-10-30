import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { KullanicilarListDto } from '../_models/kullanicilar/kullanicilarList.dto';
import { KullanicilarCreateDto } from '../_models/kullanicilar/kullanicilarCreate.dto';
import { KullanicilarService } from '../_services/kullanicilar.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormArray, FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';
import { KullaniciYetkileriService } from '../_services/kullaniciYetkileri.service';
import { KullaniciYetkileriListDto } from '../_models/kullaniciYetkileri/kullaniciYetkileriList.dto';
import { KullaniciYetkileriCreateDto } from '../_models/kullaniciYetkileri/kullaniciYetkileriCreate.dto';
import { KullaniciYetkileriDeleteDto } from '../_models/kullaniciYetkileri/kullaniciYetkileriDeletedto';
import { Authorization } from '../_constants/authorizations';
import { KullanicilarFilter } from '../_models/kullanicilar/kullanicilarFilter';
import { AffiliateDto } from '../_models/affiliate/affiliate.dto';
import { AffiliateService } from '../_services/affiliate.service';
import { AuthService } from '../_services/auth.service';
import { KullaniciIpAdresleriService } from '../_services/kullaniciIpAdresleri.service';
import { KullaniciIpAdresleriWithAvailableCount } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriWithAvailableCount.dto';
import { UpdateIpListDto } from '../_models/kullaniciIpAdresleri/updateIpList.dto';

@Component({
  selector: 'app-kullanicilar',
  templateUrl: './kullanicilar.component.html',
  styleUrls: ['./kullanicilar.component.css']
})
export class KullanicilarComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kullaniciEkleModalButton') kullaniciEkleModalButton!: ElementRef;
  @ViewChild('kullaniciEkleModalKapatButton') kullaniciEkleModalKapatButton!: ElementRef;
  @ViewChild('kullaniciSilModalKapatButton') kullaniciSilModalKapatButton!: ElementRef;

  @ViewChild('kullaniciYetkilerModalButton') kullaniciYetkilerModalButton!: ElementRef;
  @ViewChild('kullaniciYetkilerModalKapatButton') kullaniciYetkilerModalKapatButton!: ElementRef;

  @ViewChild('kullaniciIpAdresleriModalButton') kullaniciIpAdresleriModalButton!: ElementRef;
  @ViewChild('kullaniciIpAdresleriModalKapatButton') kullaniciIpAdresleriModalKapatButton!: ElementRef;

  tedarikcilerDropDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];
  affiliateDropDownModel: AffiliateDto[] = [];

  kullaniciList: KullanicilarListDto[] = [];
  kullaniciCreateModel!: KullanicilarCreateDto;
  kullaniciYetkileriModel: KullaniciYetkileriListDto[] = [];

  ipAdresForm: FormGroup;
  kullaniciIpAdresleriWithAvailableCount!: KullaniciIpAdresleriWithAvailableCount;


  filterModel!: KullanicilarFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  constructor(
    private authService: AuthService,
    private kullanicilarService: KullanicilarService,
    private kullaniciYetkileriService: KullaniciYetkileriService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriServcie: WebSiteleriService,
    private affiliateService: AffiliateService,
    private kullaniciIpAdresleriService: KullaniciIpAdresleriService,
    private alertify: AlertifyService,
    private fb: FormBuilder) {


    this.ipAdresForm = this.fb.group({
      ipAdresleri: this.fb.array([])
    });

    this.createEmptyModel();

    this.filterModel = {
      kullaniciTipi: ''
    }


  }

  ngOnInit(): void {

    if (!this.userIsAdmin()) {
      this.filterModel.kullaniciTipi = KullaniciTip.WebSiteKullanicisi.toString();
    }

    if (this.userIsAdmin()) {
      this.getTedarikciler();
      this.getAffiliates();
    }

    this.getWebSiteleri();
    this.getAll();
  }

  createEmptyModel() {
    this.kullaniciCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      tedarikciId: null,
      webSiteId: null,
      affiliateId: null,
      adSoyad: '',
      eposta: '',
      sifre: '',
      kullaniciTipi: this.userIsAdmin() ? KullaniciTip.Admin : KullaniciTip.WebSiteKullanicisi,
      ipAdresSayisi: 1,
      isOtpEnabled: false,
      durum: true
    };
  }

  getTedarikciler() {
    this.tedarikcilerService.getAll().subscribe((reponse) => {
      this.tedarikcilerDropDownModel = reponse.result;
    });
  }

  getWebSiteleri() {
    this.webSiteleriServcie.getAll().subscribe((reponse) => {
      this.webSiteleriDropDownModel = reponse.result;
    });
  }

  getAffiliates() {
    this.affiliateService.getAll().subscribe((response) => {
      this.affiliateDropDownModel = response.result;
    });
  }

  filterChanged() {
    this.getAll();
  }

  getAll() {
    this.kullanicilarService.getAll(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.kullaniciList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  closeKullaniciSilModal() {
    if (this.kullaniciSilModalKapatButton && this.kullaniciSilModalKapatButton.nativeElement) {
      this.kullaniciSilModalKapatButton.nativeElement.click();
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
      this.kullanicilarService.delete(this.selectedItemId).subscribe({
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
          this.closeKullaniciSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeKullaniciSilModal();
        }
      });
    }
  }

  get ipAdresleri(): FormArray {
    return this.ipAdresForm.get('ipAdresleri') as FormArray;
  }

  startEditIpAdresleri(itemId: string) {

    this.selectedItemId = itemId;

    this.kullaniciIpAdresleriService.getByKullaniciId(itemId).subscribe({
      next: (response) => {
        this.kullaniciIpAdresleriWithAvailableCount = response.result;

        this.ipAdresleri.clear();

        const mevcutIpAdresleri = this.kullaniciIpAdresleriWithAvailableCount.kullaniciIpAdresleriListDtos.map(x => x.ipAdresi);

        for (let i = 0; i < this.kullaniciIpAdresleriWithAvailableCount.ipAdresSayisi; i++) {
          const ipAdres = mevcutIpAdresleri[i] || ''; 
          this.ipAdresleri.push(this.fb.control(ipAdres));
        }

        this.kullaniciIpAdresleriModalButton.nativeElement.click();
      },
      error: error => {
        this.selectedItemId = "";
        alert(error);
      },
      complete: () => {
      }
    })

  }

  submitIpAdresleriForm() {
    const ipAdresleri = this.ipAdresleri.value; 
      
    const updateIpListDto: UpdateIpListDto = {
      kullaniciId: this.selectedItemId,
      ipAdresleri: ipAdresleri
    };
    
    this.isCreating = true;

    this.kullaniciIpAdresleriService.updateIpList(updateIpListDto).subscribe({
      next: response => {
        if (response.success) {
          this.closeIpAdresleriEkleModal();
          this.alertify.success("Ip adresleri güncellendi");
        }
        else {
          this.alertify.error(response.message);
        }
        this.selectedItemId = "";
        this.isCreating = false;
      },
      error: error => {
        this.selectedItemId = "";
        this.isCreating = false;
      },
      complete: () => {
        this.selectedItemId = "";
        this.isCreating = false;
      }
    });
  }

  closeIpAdresleriEkleModal() {
    if (this.kullaniciIpAdresleriModalKapatButton && this.kullaniciIpAdresleriModalKapatButton.nativeElement) {
      this.kullaniciIpAdresleriModalKapatButton.nativeElement.click();
    }
  }

  startEditYetkiler(itemId: string) {
    this.kullaniciYetkileriService.getByKullaniciId(itemId).subscribe({
      next: (response) => {

        this.kullaniciYetkileriModel = response.result;
        this.kullaniciYetkilerModalButton.nativeElement.click();
      },
      error: error => {
        alert(error);
      },
      complete: () => {
      }
    })
  }

  addRemoveYetki(item: KullaniciYetkileriListDto) {

    if (!item.kullaniciYetkiliMi) {
      let deleteModel: KullaniciYetkileriDeleteDto = {
        kullaniciId: item.kullaniciId,
        yetkiId: item.yetkiId
      };

      this.kullaniciYetkileriService.delete(deleteModel).subscribe({
        next: response => {
          if (response.success) {
            this.alertify.error("Yetki silindi");
          }
          else {
            this.alertify.error(response.message);
          }
        },
        error: error => {
        },
        complete: () => {
        }
      })

    }
    else {
      let createModel: KullaniciYetkileriCreateDto = {
        kullaniciId: item.kullaniciId,
        yetkiId: item.yetkiId
      };

      this.kullaniciYetkileriService.create(createModel).subscribe({
        next: response => {
          if (response.success) {
            this.alertify.success("Yetki eklendi");
          }
          else {
            this.alertify.error(response.message);
          }
        },
        error: error => {
        },
        complete: () => {
        }
      })
    }


  }

  startEdit(itemId: string) {
    this.kullanicilarService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.kullaniciEkleModalButton && this.kullaniciEkleModalButton.nativeElement) {
            this.kullaniciCreateModel = response.result;
            this.kullaniciEkleModalButton.nativeElement.click();
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

  closeKullaniciEkleModal() {
    if (this.kullaniciEkleModalKapatButton && this.kullaniciEkleModalKapatButton.nativeElement) {
      this.kullaniciEkleModalKapatButton.nativeElement.click();
    }
  }

  openKullaniciEkleModal() {
    this.createEmptyModel();
    this.kullaniciEkleModalButton.nativeElement.click();
  }

  submitCreateKullaniciForm(kullaniciForm: NgForm) {

    if (!kullaniciForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    if (this.userIsWebSite()) {
      this.kullaniciCreateModel.kullaniciTipi = KullaniciTip.WebSiteKullanicisi;
    }
    else {
      this.kullaniciCreateModel.kullaniciTipi = Number(this.kullaniciCreateModel.kullaniciTipi);
    }



    this.isCreating = true;

    if (this.isCreate()) {
      this.kullanicilarService.create(this.kullaniciCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKullaniciEkleModal();
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
          this.isCreating = false;
        },
        complete: () => {
          this.isCreating = false;
        }
      });
    }
    else {
      this.kullanicilarService.update(this.kullaniciCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKullaniciEkleModal();
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
          this.closeKullaniciEkleModal();
          this.isCreating = false;
        },
        complete: () => {
          this.closeKullaniciEkleModal();
          this.isCreating = false;
        }
      });
    }
  }

  closeKullaniciEkleModalKapat() {
    this.createEmptyModel();
  }

  isCreate(): boolean {
    return this.kullaniciCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

  userIsAdmin(): boolean {
    return this.authService.isAdmin();
  }

  userIsWebSite(): boolean {
    return this.authService.isWebSiteKullanicisi();
  }

  isTedarikciKullanicisi(kullaniciTip: number) {
    return kullaniciTip == KullaniciTip.TedarikciKullanicisi;
  }

  isWebSiteKullanicisi(kullaniciTip: number) {
    return kullaniciTip == KullaniciTip.WebSiteKullanicisi;
  }

  isAffiliateKullanicisi(kullaniciTip: number) {
    return kullaniciTip == KullaniciTip.AffiliateKullanicisi;
  }

}
