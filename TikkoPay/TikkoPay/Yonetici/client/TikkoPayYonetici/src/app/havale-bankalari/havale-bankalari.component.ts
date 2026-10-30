import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { HavaleBankalariService } from '../_services/havalebankalari.service';
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';
import { HavaleBankalariCreateDto } from '../_models/havalebankalari/havalebankalariCreate.dto';
import { BankaService } from '../_services/banka.service';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { HavaleBankalariPagingFilter } from '../_models/havalebankalari/havalebankalariFilter';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { HavaleBankalariDurumFilter } from '../_enums/havaleBankalariDurumFilter.enum';

@Component({
  selector: 'app-havale-bankalari',
  templateUrl: './havale-bankalari.component.html',
  styleUrls: ['./havale-bankalari.component.css']
})
export class HavaleBankalariComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('havaleBankaEkleModalButton') havaleBankaEkleModalButton!: ElementRef;
  @ViewChild('havaleBankaEkleModalKapatButton') havaleBankaEkleModalKapatButton!: ElementRef;
  @ViewChild('havaleBankaSilModalKapatButton') havaleBankaSilModalKapatButton!: ElementRef;

  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  havaleBankaList: HavaleBankalariListDto[] = [];
  havaleBankaCreateModel!: HavaleBankalariCreateDto;

  havaleBankaFilter!: HavaleBankalariPagingFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private havaleBankaService: HavaleBankalariService,
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService) {
    this.createEmptyModel();

    this.havaleBankaFilter = {
      tedarikciId: '',
      bankaId: '',
      searchText: '',
      havaleBankalariDurumFilter: HavaleBankalariDurumFilter.All,
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    this.getBankalar();
    this.getTedarikciler();

    this.getAll();
  }

  createEmptyModel() {
    this.havaleBankaCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      tedarikciId: '',
      bankaId: '',
      hesapSahibi: '',
      hesapNo: '',
      hesapMinTutar: 0,
      hesapMaxTutar: 0,
      maxYatirimAdedi: 0,
      maxYatirimToplami: 0,
      onaylananYatirimAdediTemp: 0,
      durum: true
    };
  }

  filterChanged() {
    this.getAll();
  }

  getBankalar() {
    this.bankaService.getAll().subscribe((reponse) => {
      this.bankalarDropDownModel = reponse.result;
    });
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getAll(pageNumber: number = 1) {

    this.havaleBankaFilter.pageNumber = pageNumber;

    this.havaleBankaService.getAllWithPaging(this.havaleBankaFilter).subscribe((response) => {
      if (response.success) {
        this.havaleBankaList = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  closeHavaleBankaSilModal() {
    if (this.havaleBankaSilModalKapatButton && this.havaleBankaSilModalKapatButton.nativeElement) {
      this.havaleBankaSilModalKapatButton.nativeElement.click();
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
      this.havaleBankaService.delete(this.selectedItemId).subscribe({
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
          this.closeHavaleBankaSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeHavaleBankaSilModal();
        }
      });
    }
  }

  startEdit(itemId: string) {
    this.havaleBankaService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.havaleBankaEkleModalButton && this.havaleBankaEkleModalButton.nativeElement) {
            this.havaleBankaCreateModel = response.result;
            this.havaleBankaEkleModalButton.nativeElement.click();
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

  closeHavaleBankaEkleModal() {
    if (this.havaleBankaEkleModalKapatButton && this.havaleBankaEkleModalKapatButton.nativeElement) {
      this.havaleBankaEkleModalKapatButton.nativeElement.click();
    }
  }

  openHavaleBankaEkleModal() {
    this.createEmptyModel();
    this.havaleBankaEkleModalButton.nativeElement.click();
  }

  submitCreateHavaleBankaForm(havaleBankaForm: NgForm) {
    if (!havaleBankaForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    if (this.isCreate()) {
      this.havaleBankaService.create(this.havaleBankaCreateModel).subscribe({
        next: response => {
          this.isCreating = false;

          if (response.success) {
            this.closeHavaleBankaEkleModal();
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
    else {
      this.havaleBankaService.update(this.havaleBankaCreateModel).subscribe({
        next: response => {
          this.isCreating = false;

          if (response.success) {
            this.closeHavaleBankaEkleModal();
            this.createEmptyModel();
            this.getAll();
            this.alertify.success("Kayıt güncellendi");
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

  }

  closeHavaleBankaEkleModalKapat() {
    this.createEmptyModel();
  }

  private isCreate(): boolean {
    return this.havaleBankaCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }
}
