import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { BankaService } from '../_services/banka.service';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { BankaCreateDto } from '../_models/banka/bankaCreate.dto';
import { NgForm } from '@angular/forms';
import { Authorization } from '../_constants/authorizations';

@Component({
  selector: 'app-banka',
  templateUrl: './banka.component.html',
  styleUrls: ['./banka.component.css']
})
export class BankaComponent implements OnInit {

  Authorization = Authorization;

  constructor(
    private bankaService: BankaService,
    private alertify: AlertifyService) {

  }

  @ViewChild('bankaEkleModalButton') bankaEkleModalButton!: ElementRef;
  @ViewChild('bankaEkleModalKapatButton') bankaEkleModalKapatButton!: ElementRef;
  @ViewChild('bankaSilModalKapatButton') bankaSilModalKapatButton!: ElementRef;

  bankaList: BankaListDto[] = [];
  bankaCreateModel!: BankaCreateDto;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  ngOnInit(): void {
    this.createEmptyBankaCreateModel();
    this.getAll();
  }

  createEmptyBankaCreateModel() {
    this.bankaCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      baslik: ''
    };
  }

  getAll() {
    this.bankaService.getAll().subscribe((response) => {
      if (response.success) {
        this.bankaList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  closeBankaSilModal() {
    if (this.bankaSilModalKapatButton && this.bankaSilModalKapatButton.nativeElement) {
      this.bankaSilModalKapatButton.nativeElement.click();
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
      this.bankaService.delete(this.selectedItemId).subscribe({
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
          this.closeBankaSilModal();
        }
      });
    }
  }

  startEdit(itemId: string) {
    this.bankaService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.bankaEkleModalButton && this.bankaEkleModalButton.nativeElement) {
            this.bankaCreateModel = response.result;
            this.bankaEkleModalButton.nativeElement.click();
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

  closeBankaEkleModal() {
    if (this.bankaEkleModalKapatButton && this.bankaEkleModalKapatButton.nativeElement) {
      this.bankaEkleModalKapatButton.nativeElement.click();
    }
  }

  openBankaEkleModal() {
    if (this.bankaEkleModalButton && this.bankaEkleModalButton.nativeElement) {
      this.createEmptyBankaCreateModel();
      this.bankaEkleModalButton.nativeElement.click();
    }
  }

  submitCreateBankaForm(bankaForm: NgForm) {

    if (!bankaForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    if (this.isCreate()) {
      this.bankaService.create(this.bankaCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeBankaEkleModal();
            this.createEmptyBankaCreateModel();
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
      this.bankaService.update(this.bankaCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeBankaEkleModal();
            this.createEmptyBankaCreateModel();
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

  closeBankaEkleModalKapat() {
    this.createEmptyBankaCreateModel();
  }

  private isCreate(): boolean {
    return this.bankaCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }
}
