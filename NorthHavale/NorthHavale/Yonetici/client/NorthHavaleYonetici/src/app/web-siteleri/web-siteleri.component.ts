import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { WebSiteleriCreateDto } from '../_models/websiteleri/websiteleriCreate.dto';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { NgForm } from '@angular/forms';
import { Authorization } from '../_constants/authorizations';
import { AffiliateService } from '../_services/affiliate.service';
import { AffiliateDto } from '../_models/affiliate/affiliate.dto';

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

  webSiteleri: WebSiteleriListDto[] = [];
  webSiteleriCreateModel!: WebSiteleriCreateDto;
  affiliateDronDownModel: AffiliateDto[] = [];

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  constructor(
    private affiliateService: AffiliateService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService) {

  }

  ngOnInit(): void {
    this.createEmptyWebSiteleriCreateModel();
    this.getAll();
    this.getAffiliates();
  }

  createEmptyWebSiteleriCreateModel() {
    this.webSiteleriCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      webSiteUrl: '',
      baslik: '',
      kisaBaslik: '',
      affiliateId: null,
      yaririmKomisyonOrani: 0,
      cekimKomisyonOrani: 0,
      odemeKomisyonOrani: 0,
      apiClientId: '',
      apiClientSecret: '',
      depositCallbackUrl: '',
      withdrawCallbackUrl: '',
      type: 1
    };
  }

  getAffiliates(){
    this.affiliateService.getAll().subscribe((reponse) => {
      this.affiliateDronDownModel = reponse.result;
    });
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
          console.log(error);
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
        console.log(error);
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
          this.alertify.error(error.error.message);
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
          this.alertify.error(error.error.message);
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
  

  private isCreate(): boolean {
    return this.webSiteleriCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

}
