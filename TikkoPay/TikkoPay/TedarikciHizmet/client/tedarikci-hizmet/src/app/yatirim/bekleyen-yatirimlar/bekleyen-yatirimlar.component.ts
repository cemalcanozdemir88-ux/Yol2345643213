import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { RecordsPerPage } from 'src/app/_constants/constants';
import { Durum } from 'src/app/_enums/durum.enum';
import { IslemTip } from 'src/app/_enums/islemTip.Enum';
import { YatirimCekimTalepleriDurum } from 'src/app/_enums/yatirimCekimTalepleriDurum.Enum';
import { BankaListDto } from 'src/app/_models/banka/bankaList.dto';
import { TedarikcilerListDto } from 'src/app/_models/tedarikci/tedarikcilerList.dto';
import { YatirimTalepleriListDto } from 'src/app/_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriPagingFilter } from 'src/app/_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BankaService } from 'src/app/_services/banka.service';
import { YatirimTalepleriService } from 'src/app/_services/yatirimTalepleri.service';

declare var $: any;

@Component({
  selector: 'app-bekleyen-yatirimlar',
  templateUrl: './bekleyen-yatirimlar.component.html',
  styleUrls: ['./bekleyen-yatirimlar.component.css']
})
export class BekleyenYatirimlarComponent implements OnInit, OnDestroy {
  @ViewChild('iptalModalKapatButton') iptalModalKapatButton!: ElementRef;
  @ViewChild('onayModalKapatButton') onayModalKapatButton!: ElementRef;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  public isCancelling: boolean = false;
  public isApproving: boolean = false;

  selectedItemId: string = '';
  iptalAciklama: string = '';
  selectedCancelReason: string = '';

  activePage: number = 1;
  totalPages: number = 0;

  countdownTimeValue: number = 180;
  countdownTime: number = this.countdownTimeValue;
  countdownInterval: any;
  countdownMessage: string = '';

  cancelCountDownMinuteValue: number = 10;
  countdownSubscriptions: Subscription[] = [];

  constructor(
    private yatirimTalepleriService: YatirimTalepleriService,
    private bankaService: BankaService,
    private alertify: AlertifyService
  ) {
    this.filterModel = {
      bankaId: '',
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage,
    };
  }

  ngOnInit(): void {

    this.startCountdown();

    this.getBankalar();
    this.getAll();
  }

  ngOnDestroy() {
    this.countdownSubscriptions.forEach((sub) => sub.unsubscribe());

    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }

  ngAfterViewInit() {
    $('.dateControl').datepicker({
      language: 'tr',
      keyboardNavigation: false,
      forceParse: false,
      calendarWeeks: true,
      autoclose: true,
      format: 'dd/mm/yyyy',
    });
  }

  startCountdown() {
    this.countdownInterval = setInterval(() => {
      this.countdownTime--;
      this.updateCountdownMessage();

      if (this.countdownTime <= 0) {
        clearInterval(this.countdownInterval);
        this.getAll();
        this.resetCountdown();
      }
    }, 1000);
  }

  updateCountdownMessage() {
    const minutes = Math.floor(this.countdownTime / 60);
    const seconds = this.countdownTime % 60;

    if (minutes == 0 && seconds == 0) {
      this.countdownMessage = `Kayıtlar yenileniyor`;
    } else {
      this.countdownMessage = `Kayıtlar ${minutes} dakika ${seconds} saniye sonra yenilenecektir`;
    }
  }

  resetCountdown() {
    this.countdownTime = this.countdownTimeValue;
    this.startCountdown();
  }

  getBankalar() {
    this.bankaService.getAll().subscribe((reponse) => {
      this.bankalarDropDownModel = reponse.result;
    });
  }

  filterChanged(createdDateStart: string, createdDateEnd: string) {
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getAll2(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService
      .getBekleyenYatirimTalepleriWithPaging(this.filterModel)
      .subscribe((response) => {
        if (response.success) {
          this.yatirimTalepleri = response.result;
          this.activePage = pageNumber;
          this.totalPages = response.totalPages;
        } else {
          this.alertify.error(response.message);
        }
      });
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getBekleyenYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {

        this.activePage = pageNumber;
        this.totalPages = response.totalPages;

        this.yatirimTalepleri = response.result.map((item: any) => {
          const createdDate = new Date(item.kayitTarihi);
          const now = new Date();

          const timeDifference = Math.floor((createdDate.getTime() + this.cancelCountDownMinuteValue * 60 * 1000 - now.getTime()) / 1000);
          if (timeDifference > 0) {
            item.canCancel = false;
            item.countdownMessage = this.setCountdownMessage(timeDifference);
            this.startCountdownForItem(item, timeDifference);
          } else {
            item.canCancel = true;
          }

          return item;
        });
      }
    });
  }

  startCountdownForItem(item: any, initialTime: number) {
    const sub = interval(1000).subscribe(() => {
      initialTime--;

      if (initialTime <= 0) {
        item.canCancel = true;
        item.countdownMessage = '';
        sub.unsubscribe();
      } else {
        item.countdownMessage = this.setCountdownMessage(initialTime);
      }
    });

    this.countdownSubscriptions.push(sub);
  }

  setCountdownMessage(initialTime: number) {
    const minutes = Math.floor(initialTime / 60);
    const seconds = initialTime % 60;

    return `${minutes} dk ${seconds} sn`;
  }

  downloadExcel() {
    this.yatirimTalepleriService
      .exportToExcel(this.filterModel, IslemTip.Yatirim, Durum.Bekliyor)
      .subscribe((response) => {
        if (response.success) {
          var binaryData = atob(response.result.base64String);

          const blob = new Blob(
            [
              new Uint8Array(binaryData.length).map((_, index) =>
                binaryData.charCodeAt(index)
              ),
            ],
            {
              type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
            }
          );

          const url = window.URL.createObjectURL(blob);

          const a = document.createElement('a');
          a.href = url;
          a.download = response.result.fileName;
          a.click();

          window.URL.revokeObjectURL(url);
        } else {
          this.alertify.error(response.message);
        }
      });
  }

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return 'Bekliyor';
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return 'Onaylandı';
    if (durum == YatirimCekimTalepleriDurum.Red) return 'Red';

    return '';
  }

  closeCancel() {
    this.selectedItemId = '';
    this.iptalAciklama = '';
    this.selectedCancelReason = '';
  }

  onCancelReasonChange(reason: string) {
    this.selectedCancelReason = reason;

    if (reason === 'Firma Talebi' || reason === 'İpucu Uyuşmazlığı') {
      this.iptalAciklama = reason;
    } else {
      this.iptalAciklama = '';
    }
  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
    this.iptalAciklama = '';
    this.selectedCancelReason = '';
  }

  closeIptalModal() {
    if (
      this.iptalModalKapatButton &&
      this.iptalModalKapatButton.nativeElement
    ) {
      this.iptalModalKapatButton.nativeElement.click();
    }
  }

  cancelRecord() {

    if (!this.selectedCancelReason) {
      alert("İptal nedeni seçiniz");
      return;
    }

    if (this.selectedCancelReason === 'Diğer' && this.iptalAciklama.trim() === '') {
      alert("Açıklama giriniz");
      return;
    }

    if (this.selectedItemId !== '') {
      this.isCancelling = true;

      this.yatirimTalepleriService.cancelItem(this.selectedItemId, this.iptalAciklama).subscribe({
        next: (response) => {

          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }

          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.selectedCancelReason = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        error: (error) => {
          console.log(error);

          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.selectedCancelReason = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        complete: () => {
          this.iptalAciklama = '';
          this.selectedItemId = '';
          this.selectedCancelReason = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
      });
    }
    else {
      this.isCancelling = false;
      this.closeIptalModal();
      this.alertify.error("İptal edilecek kayıt seçilmedi");
    }
  }

  closeApprove() {
    this.selectedItemId = '';
  }

  startApprove(itemId: string) {
    this.selectedItemId = itemId;
  }

  closeApproveModal() {
    if (this.onayModalKapatButton && this.onayModalKapatButton.nativeElement) {
      this.onayModalKapatButton.nativeElement.click();
    }
  }

  approveRecord() {
    if (this.selectedItemId !== '') {
      this.isApproving = true;
      this.yatirimTalepleriService.approveItem(this.selectedItemId).subscribe({
        next: (response) => {
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }
        },
        error: (error) => {
          console.log(error);
        },
        complete: () => {
          this.selectedItemId = '';
          this.isApproving = false;
          this.closeApproveModal();
        },
      });
    }
  }

}
