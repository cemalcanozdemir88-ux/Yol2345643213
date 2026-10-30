import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { TedarikciService } from '../_services/tedarikci.service';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciCreateDto } from '../_models/tedarikci/tedarikciCreate.dto';
import { NgForm } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { Authorization } from '../_constants/authorizations';

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

  tedarikciler: TedarikcilerListDto[] = [];
  tedarikciCreateModel!: TedarikciCreateDto;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  constructor(
    private tedarikciService: TedarikciService,
    private alertify: AlertifyService) {

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
      yatirimPuani: 0,
      hesapNoGosterimSayisi: 0,
      bakiyeLimitor: 0,
      durum: false
    };
  }

  getAll() {
    this.tedarikciService.getAll().subscribe((response) => {
      if (response.success) {
        this.tedarikciler = response.result;
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
          console.log(error);
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
        console.log(error);
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
          this.alertify.error(error.error.message);
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
          this.alertify.error(error.error.message);
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


