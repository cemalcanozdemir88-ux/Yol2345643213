import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription, interval } from 'rxjs';
import { RecordsPerPage } from 'src/app/_constants/constants';
import { Durum } from 'src/app/_enums/durum.enum';
import { IslemTip } from 'src/app/_enums/islemTip.Enum';
import { YatirimCekimTalepleriDurum } from 'src/app/_enums/yatirimCekimTalepleriDurum.Enum';
import { BankaListDto } from 'src/app/_models/banka/bankaList.dto';
import { TedarikcilerListDto } from 'src/app/_models/tedarikci/tedarikcilerList.dto';
import { YatirimTalepleriPagingFilter } from 'src/app/_models/yatirimTalepleri/yatirimTalepleriFilter';
import { YatirimTalepleriManualListDto } from 'src/app/_models/yatirimTalepleriManual/yatirimTaleperliManualList.dto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BankaService } from 'src/app/_services/banka.service';
import { SignalrService } from 'src/app/_services/signalr.service';
import { YatirimTalepleriManualService } from 'src/app/_services/yatirimTalepleriManual.service';

declare var $: any;

@Component({
  selector: 'app-bekleyen-yatirimlar-manual',
  templateUrl: './bekleyen-yatirimlar-manual.component.html',
  styleUrls: ['./bekleyen-yatirimlar-manual.component.css']
})
export class BekleyenYatirimlarManualComponent implements OnInit, OnDestroy {
  @ViewChild('iptalModalKapatButton') iptalModalKapatButton!: ElementRef;
  @ViewChild('onayModalKapatButton') onayModalKapatButton!: ElementRef;

  yatirimTalepleri: YatirimTalepleriManualListDto[] = [];
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

  private _sub?: Subscription;

  constructor(
    private yatirimTalepleriService: YatirimTalepleriManualService,
    private bankaService: BankaService,
    private signalr: SignalrService,
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
      recordsPerPage: 9999999,
    };
  }

  ngOnInit(): void {

    this._sub = this.signalr.yeniYatirimTalebi$.subscribe(order => {
      if (order) {
        this.yatirimTalepleri.push(order);
        //this.sortByKayitTarihi();
      }
    });

    this.getBankalar();
    this.getAll();

  }

  ngOnDestroy() {
    this._sub?.unsubscribe();
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

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getBekleyenYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {

        this.yatirimTalepleri = response.result;
        //this.sortByKayitTarihi();
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
    });
  }

  private sortByKayitTarihi() {
    this.yatirimTalepleri.sort((a: any, b: any) => {
      const da = a && a.kayitTarihi ? new Date(a.kayitTarihi) : new Date(0);
      const db = b && b.kayitTarihi ? new Date(b.kayitTarihi) : new Date(0);
      return da.getTime() - db.getTime();
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

  onCancelReasonChange(reason: string) {
    this.selectedCancelReason = reason;

    if (reason === 'Firma Talebi' || reason === 'İpucu Uyuşmazlığı') {
      this.iptalAciklama = reason;
    } else {
      this.iptalAciklama = '';
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
          this.selectedCancelReason = '';
          this.selectedItemId = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        error: (error) => {
          console.log(error);

          this.iptalAciklama = '';
          this.selectedCancelReason = '';
          this.selectedItemId = '';
          this.isCancelling = false;
          this.closeIptalModal();
        },
        complete: () => {
          this.iptalAciklama = '';
          this.selectedCancelReason = '';
          this.selectedItemId = '';
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
