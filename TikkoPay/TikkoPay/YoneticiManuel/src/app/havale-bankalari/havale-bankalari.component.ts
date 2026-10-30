import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { HavaleBankalariService } from '../_services/havalebankalari.service';
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';
import { HavaleBankalariPagingFilter } from '../_models/havalebankalari/havalebankalariFilter';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { HavaleBankalariDurumFilter } from '../_enums/havaleBankalariDurumFilter.enum';
import { YatirimManuelCreateDto } from '../_models/yatirimTalepleri/yatirimManuelCreate.dto';
import { ManuelYatirimTalepleriService } from '../_services/manuelYatirimTalepleri.service';
import { SignalrService } from '../_services/signalr.service';
import { Subscription } from 'rxjs';

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

  havaleBankaList: HavaleBankalariListDto[] = [];

  yatirimManuelCreateModel!: YatirimManuelCreateDto;

  havaleBankaFilter!: HavaleBankalariPagingFilter;

  public isCreating: boolean = false;

  selectedItemId: string = "";

  private _sub?: Subscription;

  constructor(
    private havaleBankaService: HavaleBankalariService,
    private maneulYatirimTalepleriService: ManuelYatirimTalepleriService,
    private signalr: SignalrService,
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

    this._sub = this.signalr.updateHavaleBankaList$.subscribe(() => {
      this.getAll();
    });

    this.getAll();
  }

  ngOnDestroy() {
    this._sub?.unsubscribe();
  }

  createEmptyModel() {
    this.yatirimManuelCreateModel = {
      havaleBankaId: '',
      adSoyad: '',
      userName: '',
      userId: '',
      tutar: 0
    };
  }

  filterChanged() {
    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.havaleBankaFilter.pageNumber = pageNumber;

    this.havaleBankaService.getAll(this.havaleBankaFilter).subscribe((response) => {
      if (response.success) {
        this.havaleBankaList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  startEdit(itemId: string) {

    this.selectedItemId = itemId;

    if (this.havaleBankaEkleModalButton && this.havaleBankaEkleModalButton.nativeElement) {
      this.havaleBankaEkleModalButton.nativeElement.click();
    }
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

  submitManuelYatirimEkleForm(yatirimEkleForm: NgForm) {
    if (!yatirimEkleForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.yatirimManuelCreateModel.havaleBankaId = this.selectedItemId;

    this.isCreating = true;

    this.maneulYatirimTalepleriService.createYatirim(this.yatirimManuelCreateModel).subscribe({
      next: response => {
        this.isCreating = false;

        if (response.success) {
          this.closeHavaleBankaEkleModal();
          this.createEmptyModel();
          this.alertify.success("Yatırım talebi başarıyla oluşturuldu.");
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

  closeHavaleBankaEkleModalKapat() {
    this.createEmptyModel();
  }

  copyHesap(item: HavaleBankalariListDto) {
    const text = `Alıcı Adı Soyadı\n${item.hesapSahibi}\n\nAlıcı Iban\n${item.hesapNo}`;

    if (navigator && (navigator as any).clipboard && (navigator as any).clipboard.writeText) {
      (navigator as any).clipboard.writeText(text).then(() => {
        this.alertify.success('Hesap bilgileri panoya kopyalandı.');
      }).catch(() => {
        this.alertify.error('Panoya kopyalanamadı.');
      });
    } else {
      const textarea = document.createElement('textarea');
      textarea.value = text;
      textarea.style.position = 'fixed';
      textarea.style.left = '-9999px';
      document.body.appendChild(textarea);
      textarea.select();
      try {
        document.execCommand('copy');
        this.alertify.success('Hesap bilgileri panoya kopyalandı.');
      } catch (e) {
        this.alertify.error('Panoya kopyalanamadı.');
      }
      document.body.removeChild(textarea);
    }
  }

}
