import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { AlertifyService } from '../_services/alertify.service';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { KullaniciBlacklistFilter } from '../_models/kullaniciBlacklist/kullaniciBlacklistFilter.dto';
import { Authorization } from '../_constants/authorizations';
import { RecordsPerPage } from '../_constants/constants';
import { KullaniciBlacklistDto } from '../_models/kullaniciBlacklist/kullaniciBlacklist.dto';
import { KullaniciBlacklistCreateDto } from '../_models/kullaniciBlacklist/kullaniciBlacklistCreate.dto';
import { KullaniciBlacklistService } from '../_services/kullaniciBlacklist.service';
import { NgForm } from '@angular/forms';


@Component({
  selector: 'app-kullanici-blacklist',
  templateUrl: './kullanici-blacklist.component.html',
  styleUrls: []
})
export class KullaniciBlacklistComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kullaniciBlacklistEkleModalButton') kullaniciBlacklistEkleModalButton!: ElementRef;
  @ViewChild('kullaniciBlacklistEkleModalKapatButton') kullaniciBlacklistEkleModalKapatButton!: ElementRef;
  @ViewChild('kullaniciBlacklistSilModalKapatButton') kullaniciBlacklistSilModalKapatButton!: ElementRef;

  webSiteDropDownModel: WebSiteleriListDto[] = [];

  kullaniciBlackList: KullaniciBlacklistDto[] = [];
  kullaniciBlacklistCreateModel!: KullaniciBlacklistCreateDto;

  kullaniciBlacklistFilter!: KullaniciBlacklistFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private kullaniciBlacklistService: KullaniciBlacklistService,
    private webSiteService: WebSiteleriService,
    private alertify: AlertifyService) {

    this.createEmptyModel();

    this.kullaniciBlacklistFilter = {
      websiteId: '',
      userId: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }
  }

  ngOnInit(): void {
    this.getWebsiteleri();
    this.getAll();
  }

  createEmptyModel() {
    this.kullaniciBlacklistCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      websiteId: '',
      userId: 0,
    };
  }

  filterChanged() {
    this.getAll();
  }

  getWebsiteleri() {
    this.webSiteService.getAll().subscribe((response) => {
      this.webSiteDropDownModel = response.result;
    })
  }

  getAll(pageNumber: number = 1) {
    this.kullaniciBlacklistFilter.pageNumber = pageNumber;

    this.kullaniciBlacklistService.getAllWithPaging(this.kullaniciBlacklistFilter).subscribe((response) => {
      if (response.success) {
        this.kullaniciBlackList = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }
    })

  }

  closeKullaniciBlacklistSilModal() {
    if (this.kullaniciBlacklistSilModalKapatButton && this.kullaniciBlacklistSilModalKapatButton.nativeElement) {
      this.kullaniciBlacklistSilModalKapatButton.nativeElement.click();
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
      this.kullaniciBlacklistService.delete(this.selectedItemId).subscribe({
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
          this.closeKullaniciBlacklistSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeKullaniciBlacklistSilModal();
        }
      });
    }
  }

  closeKullaniciBlacklistEkleModal() {
    if (this.kullaniciBlacklistEkleModalKapatButton && this.kullaniciBlacklistEkleModalKapatButton.nativeElement) {
      this.kullaniciBlacklistEkleModalKapatButton.nativeElement.click();
    }
  }

  openKullaniciBlacklistEkleModal() {
    this.createEmptyModel();
    this.kullaniciBlacklistEkleModalButton.nativeElement.click();
  }

  submitCreateKullaniciBlacklistForm(havaleBankaForm: NgForm) {
    if (!havaleBankaForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    this.kullaniciBlacklistService.create(this.kullaniciBlacklistCreateModel).subscribe({
      next: response => {
        this.isCreating = false;

        if (response.success) {
          this.closeKullaniciBlacklistEkleModal();
          this.createEmptyModel();
          this.getAll();
          this.alertify.success("Kayıt eklendi");
        }
        else {
          this.alertify.error(response.message);
        }
      },
      error: error => {
        this.isCreating = false;
      },
      complete: () => {

      }
    });

  }

  closeKullaniciBlacklistEkleModalKapat() {
    this.createEmptyModel();
  }

  private isCreate(): boolean {
    return this.kullaniciBlacklistCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

}
