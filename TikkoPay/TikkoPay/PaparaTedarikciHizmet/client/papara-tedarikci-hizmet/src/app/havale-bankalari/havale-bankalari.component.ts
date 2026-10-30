import { Component, ElementRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { HavaleBankalariCreateDto } from '../_models/havalebankalari/havalebankalariCreate.dto';
import { HavaleBankalariFilter, HavaleBankalariPagingFilter } from '../_models/havalebankalari/havalebankalariFilter';
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { AlertifyService } from '../_services/alertify.service';
import { BankaService } from '../_services/banka.service';
import { HavaleBankalariService } from '../_services/havalebankalari.service';
import { RecordsPerPage } from '../_constants/constants';
import { HavaleBankalariDurumFilter } from '../_enums/havaleBankalariDurumFilter.enum';

@Component({
  selector: 'app-havale-bankalari',
  templateUrl: './havale-bankalari.component.html',
  styleUrls: ['./havale-bankalari.component.css']
})
export class HavaleBankalariComponent {

  @ViewChild('havaleBankaEkleModalButton') havaleBankaEkleModalButton!: ElementRef;
  @ViewChild('havaleBankaEkleModalKapatButton') havaleBankaEkleModalKapatButton!: ElementRef;
  @ViewChild('havaleBankaSilModalKapatButton') havaleBankaSilModalKapatButton!: ElementRef;

  bankalarDropDownModel: BankaListDto[] = [];

  havaleBankaList: HavaleBankalariListDto[] = [];
  havaleBankaCreateModel!: HavaleBankalariCreateDto;

  havaleBankaFilter!: HavaleBankalariPagingFilter;

  public isCreating: boolean = false;
  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private havaleBankaService: HavaleBankalariService,
    private bankaService: BankaService,
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
    this.getAll();
  }

  createEmptyModel() {
    this.havaleBankaCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      tedarikciId: '00000000-0000-0000-0000-000000000000',
      bankaId: '',
      hesapSahibi: '',
      hesapNo: '',
      hesapMinTutar: 0,
      hesapMaxTutar: 0,
      maxYatirimAdedi: 0,
      maxYatirimToplami: 0,
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
        console.log(error);
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
          if (response.success) {
            this.closeHavaleBankaEkleModal();
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
          this.alertify.error(error.error.message);
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }
    else {
      this.havaleBankaService.update(this.havaleBankaCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeHavaleBankaEkleModal();
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
          this.alertify.error(error.error.message);
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

  public isCreate(): boolean {
    return this.havaleBankaCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }
}
