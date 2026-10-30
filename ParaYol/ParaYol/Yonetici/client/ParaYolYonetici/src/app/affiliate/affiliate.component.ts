import { Component, ElementRef, ViewChild } from '@angular/core';
import { AffiliateDto } from '../_models/affiliate/affiliate.dto';
import { AffiliateCreateDto } from '../_models/affiliate/affiliateCreate.dto';
import { AffiliateService } from '../_services/affiliate.service';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { Authorization } from '../_constants/authorizations';

@Component({
  selector: 'app-affiliate',
  templateUrl: './affiliate.component.html',
  styleUrls: ['./affiliate.component.css']
})
export class AffiliateComponent {

  Authorization = Authorization;

  @ViewChild('affiliateEkleModalButton') affiliateEkleModalButton!: ElementRef;
  @ViewChild('affiliateEkleModalKapatButton') affiliateEkleModalKapatButton!: ElementRef;
  @ViewChild('affiliateSilModalKapatButton') affiliateSilModalKapatButton!: ElementRef;

  affiliates: AffiliateDto[] = [];
  affiliateCreateModel!: AffiliateCreateDto;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  constructor(
    private affiliateService: AffiliateService,
    private alertify: AlertifyService) {

  }

  ngOnInit(): void {
    this.createEmptyAffiliateCreateModel();
    this.getAll();
  }

  createEmptyAffiliateCreateModel() {
    this.affiliateCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      unvan: '',
      yaririmKomisyonOrani: 0,
      cekimKomisyonOrani: 0,
    }
  }

  getAll() {
    this.affiliateService.getAll().subscribe((response) => {
      if (response.success) {
        this.affiliates = response.result;
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
      this.affiliateService.delete(this.selectedItemId).subscribe({
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
          this.closeAffiliateSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeAffiliateSilModal();
        }
      });
    }
  }

  closeAffiliateSilModal() {
    if (this.affiliateSilModalKapatButton && this.affiliateSilModalKapatButton.nativeElement) {
      this.affiliateSilModalKapatButton.nativeElement.click();
    }
  }

  startEdit(itemId: string) {
    this.affiliateService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.affiliateEkleModalButton && this.affiliateEkleModalButton.nativeElement) {
            this.affiliateCreateModel = response.result;
            this.affiliateEkleModalButton.nativeElement.click();
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

  closeAffiliateEkleModal() {
    if (this.affiliateEkleModalKapatButton && this.affiliateEkleModalKapatButton.nativeElement) {
      this.affiliateEkleModalKapatButton.nativeElement.click();
    }
  }

  openAffiliateEkleModal() {
    if (this.affiliateEkleModalButton && this.affiliateEkleModalButton.nativeElement) {
      this.createEmptyAffiliateCreateModel();
      this.affiliateEkleModalButton.nativeElement.click();
    }
  }

  submitCreateAffiliateForm(affiliateForm: NgForm) {

    if (!affiliateForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    if (this.isCreate()) {
      this.affiliateService.create(this.affiliateCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeAffiliateEkleModal();
            this.createEmptyAffiliateCreateModel();
            this.getAll();
            this.alertify.success("Affiliate eklendi");
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
      this.affiliateService.update(this.affiliateCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeAffiliateEkleModal();
            this.createEmptyAffiliateCreateModel();
            this.getAll();
            this.alertify.success("Affiliate güncellendi");
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

  closeAffiliateEkleModalKapat() {
    this.createEmptyAffiliateCreateModel();
  }

  private isCreate(): boolean {
    return this.affiliateCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

}
