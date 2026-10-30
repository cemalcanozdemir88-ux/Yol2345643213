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
  @ViewChild('dekontDosyasi') fileInput!: ElementRef;
  @ViewChild('fileUploadArea') fileUploadArea!: ElementRef;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  public isCancelling: boolean = false;
  public isApproving: boolean = false;
  public isDragging: boolean = false;
  public isFocused: boolean = false;

  selectedItemId: string = "";
  iptalAciklama: string = '';
  selectedCancelReason: string = '';
  onayAciklama: string = '';

  selectedFiles: File[] = [];
  selectedFileNames: string[] = [];

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
      recordsPerPage: RecordsPerPage
    };
  }

  ngOnInit(): void {
    this.startCountdown();
    this.getBankalar();
    this.getAll();

    // Document düzeyinde paste olayını dinle
    document.addEventListener('paste', this.handlePasteEvent);
  }

  ngOnDestroy() {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
    // Paste olay dinleyicisini temizle
    document.removeEventListener('paste', this.handlePasteEvent);
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

  // Benzersiz dosya ismi oluşturma
  private generateUniqueFileName(originalName: string): string {
    const extension = originalName.split('.').pop() || 'png'; // Uzantıyı al, yoksa varsayılan olarak 'png' kullan
    const uniqueId = this.generateGuid(); // Benzersiz bir GUID oluştur
    return `${uniqueId}.${extension}`; // Örneğin: "550e8400-e29b-41d4-a716-446655440000.png"
  }


  private generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = crypto.getRandomValues(new Uint8Array(1))[0] % 16;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }


  // Paste olayını işleyen metod
  private handlePasteEvent = (event: ClipboardEvent) => {
    // Popup açık değilse paste olayını yoksay
    const modal = document.getElementById('onayModal');
    if (!modal || !modal.classList.contains('show')) {
      return;
    }

    event.preventDefault();
    if (!event.clipboardData) {
      this.alertify.error('Panoya erişim sağlanamadı. Tarayıcı izinlerini kontrol edin.');
      return;
    }
    const items = event.clipboardData.items;
    let files: File[] = [];
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      if (item.kind === 'file' && item.type.startsWith('image/')) {
        const file = item.getAsFile();
        if (file) {
          // Dosya ismini benzersiz bir GUID ile değiştir
          const uniqueFileName = this.generateUniqueFileName(file.name);
          const renamedFile = new File([file], uniqueFileName, { type: file.type });
          files.push(renamedFile);
        }
      } else if (item.type.startsWith('image/')) {
        const file = item.getAsFile();
        if (file) {
          // Dosya ismini benzersiz bir GUID ile değiştir
          const uniqueFileName = this.generateUniqueFileName(file.name);
          const renamedFile = new File([file], uniqueFileName, { type: file.type });
          files.push(renamedFile);
        }
      }
    }
    if (files.length > 0) {
      this.addFiles(files);
    } else {
      this.alertify.error('Panoda uygun bir resim dosyası bulunamadı. Lütfen bir resim kopyalayın ve tekrar deneyin.');
    }
  };

  // Yapıştır butonu için metod
  async pasteFromClipboard() {
    try {
      const clipboardItems = await navigator.clipboard.read();
      let files: File[] = [];
      for (const item of clipboardItems) {
        for (const type of item.types) {
          if (type.startsWith('image/')) {
            const blob = await item.getType(type);
            // Dosya ismini benzersiz bir GUID ile değiştir
            const uniqueFileName = this.generateUniqueFileName(`pasted-image.${type.split('/')[1]}`);
            const file = new File([blob], uniqueFileName, { type });
            files.push(file);
          }
        }
      }
      if (files.length > 0) {
        this.addFiles(files);
      } else {
        this.alertify.error('Panoda uygun bir resim dosyası bulunamadı. Lütfen bir resim kopyalayın ve tekrar deneyin.');
      }
    } catch (err) {
      this.alertify.error('Panoya erişim sağlanamadı. Tarayıcı izinlerini kontrol edin veya dosyayı manuel olarak seçin.');
      console.error('Panoya erişim hatası:', err);
    }
  }

  private addFiles(files: File[]) {
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'application/pdf'];
    const maxFileSize = 5 * 1024 * 1024; // 5 MB

    for (const file of files) {
      if (!allowedTypes.includes(file.type)) {
        this.alertify.error(`"${file.name}" dosyası yüklenemedi: Yalnızca JPG, JPEG, PNG veya PDF dosyaları yüklenebilir.`);
        continue;
      }

      if (file.size > maxFileSize) {
        this.alertify.error(`"${file.name}" dosyası yüklenemedi: Dosya boyutu 5 MB'dan büyük olamaz.`);
        continue;
      }

      // Aynı dosyanın zaten eklenip eklenmediğini kontrol et
      const isDuplicate = this.selectedFiles.some(
        existingFile => existingFile.size === file.size && existingFile.name === file.name && existingFile.type === file.type
      );
      if (!isDuplicate) {
        this.selectedFiles.push(file);
        this.selectedFileNames.push(file.name);
      } else {
        this.alertify.error(`"${file.name}" dosyası zaten eklenmiş.`);
      }
    }
  }


  // Listeden dosya kaldırma
  removeFile(index: number) {
    this.selectedFiles.splice(index, 1);
    this.selectedFileNames.splice(index, 1);
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

  getAll(pageNumber: number = 1) {
    this.filterModel.pageNumber = pageNumber;
    this.cekimTalepleriService.getBekleyenCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.yatirimTalepleri = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      } else {
        this.alertify.error(response.message);
      }
    });
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
      } else {
        this.alertify.error(response.message);
      }
    });
  }

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";
    return "";
  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
    this.iptalAciklama = '';
    this.selectedCancelReason = '';
  }

  closeCancel() {
    this.selectedItemId = "";
    this.iptalAciklama = '';
    this.selectedCancelReason = '';
  }

  closeIptalModal() {
    if (this.iptalModalKapatButton && this.iptalModalKapatButton.nativeElement) {
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

    if (this.selectedItemId !== "") {
      this.isCancelling = true;

      this.cekimTalepleriService.cancelItem(this.selectedItemId, this.iptalAciklama).subscribe({
        next: (response) => {
          this.getAll();
          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }
          this.iptalAciklama = '';
          this.selectedCancelReason = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        },
        error: error => {
          console.log(error);
          this.iptalAciklama = '';
          this.selectedCancelReason = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        },
        complete: () => {
          this.iptalAciklama = '';
          this.selectedCancelReason = '';
          this.selectedItemId = "";
          this.isCancelling = false;
          this.closeIptalModal();
        }
      });
    }
  }

  startApprove(itemId: string) {
    this.selectedItemId = itemId;
    this.selectedFiles = [];
    this.selectedFileNames = [];
    this.isFocused = false;
  }

  onFocus() {
    this.isFocused = true;
  }

  onBlur() {
    this.isFocused = false;
  }

  closeApprove() {
    this.selectedItemId = "";
    this.selectedFiles = [];
    this.selectedFileNames = [];
    this.isFocused = false;
  }

  closeApproveModal() {
    if (this.onayModalKapatButton && this.onayModalKapatButton.nativeElement) {
      this.onayModalKapatButton.nativeElement.click();
    }
  }

  triggerFileInput() {
    if (this.fileInput && this.fileInput.nativeElement) {
      this.fileInput.nativeElement.click();
    } else {
      console.error('File input reference is not available.');
    }
  }

  onFileSelected(event: any) {
    const files: File[] = Array.from(event.target.files);
    if (files.length > 0) {
      this.addFiles(files);
      event.target.value = ''; // Dosya girişini sıfırla
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const fileArray: File[] = Array.from(files);
      this.addFiles(fileArray);
    }
  }

  approveRecord() {
    if (this.selectedFiles.length === 0) {
      alert("En az bir dosya seçiniz!");
      return;
    }

    if (this.selectedItemId !== "") {
      this.isApproving = true;

      const formData = new FormData();
      formData.append('id', this.selectedItemId);
      this.selectedFiles.forEach(file => {
        formData.append('dekontDosyasi', file, file.name);
      });

      this.cekimTalepleriService.approveItem(this.selectedItemId, formData).subscribe({
        next: (response) => {
          this.getAll();
          if (response.success) {
            this.alertify.success(response.message);
          } else {
            this.alertify.error(response.message);
          }
          this.onayAciklama = "";
          this.selectedItemId = "";
          this.selectedFiles = [];
          this.selectedFileNames = [];
          this.isApproving = false;
          this.closeApproveModal();
        },
        error: error => {
          console.log(error);
          this.onayAciklama = "";
          this.selectedItemId = "";
          this.selectedFiles = [];
          this.selectedFileNames = [];
          this.isApproving = false;
          this.closeApproveModal();
        },
        complete: () => {
          this.onayAciklama = "";
          this.selectedItemId = "";
          this.selectedFiles = [];
          this.selectedFileNames = [];
          this.isApproving = false;
          this.closeApproveModal();
        }
      });
    }
  }
}