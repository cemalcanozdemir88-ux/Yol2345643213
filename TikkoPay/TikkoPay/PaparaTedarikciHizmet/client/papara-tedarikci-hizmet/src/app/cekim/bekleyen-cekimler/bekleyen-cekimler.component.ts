import { Component, ElementRef, ViewChild } from '@angular/core';
import { RecordsPerPage } from 'src/app/_constants/constants';
import { Durum } from 'src/app/_enums/durum.enum';
import { IslemTip } from 'src/app/_enums/islemTip.Enum';
import { YatirimCekimTalepleriDurum } from 'src/app/_enums/yatirimCekimTalepleriDurum.Enum';
import { BankaListDto } from 'src/app/_models/banka/bankaList.dto';
import { YatirimTalepleriListDto } from 'src/app/_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriPagingFilter } from 'src/app/_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { BankaService } from 'src/app/_services/banka.service';
import { CekimTalepleriService } from 'src/app/_services/cekimTalepleri.service';

declare var $: any;

@Component({
  selector: 'app-bekleyen-cekimler',
  templateUrl: './bekleyen-cekimler.component.html',
  styleUrls: ['./bekleyen-cekimler.component.css']
})
export class BekleyenCekimlerComponent {

  @ViewChild('iptalModalKapatButton') iptalModalKapatButton!: ElementRef;
  @ViewChild('onayModalKapatButton') onayModalKapatButton!: ElementRef;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  public isCancelling: boolean = false;
  public isApproving: boolean = false;

  selectedItemId: string = "";
  iptalAciklama: string = '';
  onayAciklama: string = '';

  activePage: number = 1;
  totalPages: number = 0;

  countdownTimeValue: number = 180;
  countdownTime: number = this.countdownTimeValue; 
  countdownInterval: any;
  countdownMessage: string = '';

  constructor(
    private cekimTalepleriService: CekimTalepleriService,
    private bankaService: BankaService,
    private authService: AuthService,
    private alertify: AlertifyService) {

    this.filterModel = {
      bankaId: '',
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {

    this.startCountdown();

    this.getBankalar();
    this.getAll();
  }

  ngOnDestroy() {
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
      format: "dd/mm/yyyy"
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
    
    if(minutes == 0 && seconds == 0){
      this.countdownMessage = `Kayıtlar yenileniyor`;
    }else {
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

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.cekimTalepleriService.getBekleyenCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.yatirimTalepleri = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  downloadExcel() {
    this.cekimTalepleriService.exportToExcel(this.filterModel, IslemTip.Cekim, Durum.Bekliyor).subscribe((response) => {
      if (response.success) {

        var binaryData = atob(response.result.base64String);

        const blob = new Blob([new Uint8Array(binaryData.length).map((_, index) => binaryData.charCodeAt(index))], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = response.result.fileName;
        a.click();

        window.URL.revokeObjectURL(url);

      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";

    return "";
  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
  }

  closeCancel() {
    this.selectedItemId = "";
  }

  closeIptalModal() {
    if (this.iptalModalKapatButton && this.iptalModalKapatButton.nativeElement) {
      this.iptalModalKapatButton.nativeElement.click();
    }
  }

  cancelRecord() {

    if(this.iptalAciklama == ''){
      alert("Açıklama giriniz");
      return;
    }

    if (this.selectedItemId !== "") {
      this.isCancelling = true;

      this.cekimTalepleriService.cancelItem(this.selectedItemId, this.iptalAciklama).subscribe({
        next: (response) => {
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          }
          else {
            this.alertify.error(response.message);
          }

          this.iptalAciklama = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        },
        error: error => {
          console.log(error);

          this.alertify.error(error.error.message);

          this.iptalAciklama = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        },
        complete: () => {
          this.iptalAciklama = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        }
      });
    }
  }

  startApprove(itemId: string) {
    this.selectedItemId = itemId;
  }

  closeApprove() {
    this.selectedItemId = "";
  }

  closeApproveModal() {
    if (this.onayModalKapatButton && this.onayModalKapatButton.nativeElement) {
      this.onayModalKapatButton.nativeElement.click();
    }
  }

  approveRecord() {

    if(this.onayAciklama == ''){
      alert("Açıklama giriniz");
      return;
    }

    if (this.selectedItemId !== "") {
      this.isApproving = true;
      
      this.cekimTalepleriService.approveItem(this.selectedItemId, this.onayAciklama).subscribe({
        next: (response) => {
          this.getAll();
          
          if (response.success) {
            this.alertify.success(response.message);
          }
          else {
            this.alertify.error(response.message);
          }

          this.onayAciklama = "";
          this.selectedItemId = "";
          this.isApproving = false;
          this.closeApproveModal();
        },
        error: error => {
          console.log(error);
          this.alertify.error(error.error.message);

          this.onayAciklama = "";
          this.selectedItemId = "";
          this.isApproving = false;
          this.closeApproveModal();
        },
        complete: () => {
          this.onayAciklama = "";
          this.selectedItemId = "";
          this.isApproving = false;
          this.closeApproveModal();
        }
      });
    }
  }
}
