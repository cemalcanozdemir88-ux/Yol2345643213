import { Component, ElementRef, ViewChild } from '@angular/core';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { KullanicilarListDto } from '../_models/kullanicilar/kullanicilarList.dto';
import { KullanicilarCreateDto } from '../_models/kullanicilar/kullanicilarCreate.dto';
import { KullanicilarService } from '../_services/kullanicilar.service';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';
import { KullanicilarFilter } from '../_models/kullanicilar/kullanicilarFilter';

@Component({
  selector: 'app-tedarikci-kullanici-yonetimi',
  templateUrl: './tedarikci-kullanici-yonetimi.component.html',
  styleUrls: ['./tedarikci-kullanici-yonetimi.component.css']
})
export class TedarikciKullaniciYonetimiComponent {

  @ViewChild('kullaniciEkleModalButton') kullaniciEkleModalButton!: ElementRef;
  @ViewChild('kullaniciEkleModalKapatButton') kullaniciEkleModalKapatButton!: ElementRef;
  @ViewChild('kullaniciSilModalKapatButton') kullaniciSilModalKapatButton!: ElementRef;

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  kullaniciList: KullanicilarListDto[] = [];
  kullaniciCreateModel!: KullanicilarCreateDto;

  filterModel!: KullanicilarFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;
  selectedItemId: string = "";

  constructor(
    private kullanicilarService: KullanicilarService,
    private alertify: AlertifyService) {

    this.createEmptyModel();

    this.filterModel = {
      kullaniciTipi: ''
    }
  }

  ngOnInit(): void {
    this.getAll();
  }

  createEmptyModel() {
    this.kullaniciCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      tedarikciId: '00000000-0000-0000-0000-000000000000',
      webSiteId: null,
      adSoyad: '',
      eposta: '',
      sifre: '',
      kullaniciTipi: KullaniciTip.TedarikciKullanicisi,
      durum: true
    };
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
          alert(error);
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeKullaniciSilModal();
        }
      });
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

    this.isCreating = true;

    this.kullaniciCreateModel.kullaniciTipi = Number(this.kullaniciCreateModel.kullaniciTipi);

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
          
          this.isCreating = false;

        },
        complete: () => {
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
}
