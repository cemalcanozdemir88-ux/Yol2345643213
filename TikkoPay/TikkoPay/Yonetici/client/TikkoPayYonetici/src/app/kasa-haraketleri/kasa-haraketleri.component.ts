import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Durum } from '../_enums/durum.enum';
import { KasaHaraketleriCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriCreate.dto';
import { KasaHaraketleriFilter, KasaHaraketleriPagingFilter } from '../_models/kasaHaraketleri/kasaHaraketleriFilter';
import { KasaHaraketleriListDto } from '../_models/kasaHaraketleri/kasaHaraketleriList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { AlertifyService } from '../_services/alertify.service';
import { KasaHaraketleriService } from '../_services/kasaharaketleri.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { RecordsPerPage } from '../_constants/constants';
import { Authorization } from '../_constants/authorizations';
import { KasaHaraketleriVirmanCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriVirmanCreate.dto';
import { KasaHaraketleriVirmanService } from '../_services/kasaharaketleriVirman.service';

declare var $: any;

@Component({
  selector: 'app-kasa-haraketleri',
  templateUrl: './kasa-haraketleri.component.html',
  styleUrls: ['./kasa-haraketleri.component.css']
})
export class KasaHaraketleriComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kasaHaraketleriEkleModalButton') kasaHaraketleriEkleModalButton!: ElementRef;
  @ViewChild('kasaHaraketleriEkleModalKapatButton') kasaHaraketleriEkleModalKapatButton!: ElementRef;
  @ViewChild('kasaHaraketleriSilModalKapatButton') kasaHaraketleriSilModalKapatButton!: ElementRef;

  @ViewChild('virmanEkleModalButton') virmanEkleModalButton!: ElementRef;
  @ViewChild('virmanEkleModalKapatButton') virmanEkleModalKapatButton!: ElementRef;

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  kasaHaraketleriList: KasaHaraketleriListDto[] = [];
  kasaHaraketleriCreateModel!: KasaHaraketleriCreateDto;

  kasaHaraketleriVirmanCreateModel!: KasaHaraketleriVirmanCreateDto;

  kasaHaraketleriFilter!: KasaHaraketleriPagingFilter;

  public isDeleting: boolean = false;
  public isCreating: boolean = false;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private kasaHaraketleriService: KasaHaraketleriService,
    private kasaHaraketleriVirmanService: KasaHaraketleriVirmanService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService) {
    this.createEmptyModel();
    this.createEmptyVirmanModel();

    this.kasaHaraketleriFilter = {
      tedarikciId: '',
      durum: '',
      islemTipi: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    this.getTedarikciler();

    this.getAll();
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

  createEmptyModel() {
    this.kasaHaraketleriCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
      tedarikciId: '',
      aciklama: '',
      tutar: 0,
      dovizKuru: 0,
      usdtMiktar: 0,
      usdtAdres: '',
      usdtTxtNo: '',
      resimUrl: '',
      islemTipi: 1,
      durum: 1
    };
  }

  createEmptyVirmanModel() {
    this.kasaHaraketleriVirmanCreateModel = {
      fromTedarikciId: '00000000-0000-0000-0000-000000000000',
      fromVirmanIslemTipi: 1,
      toTedarikciId: '00000000-0000-0000-0000-000000000000',
      toVirmanIslemTipi: -1,
      tutar: 0,
      dovizKuru: 0,
      usdtMiktar: 0,
      usdtAdres: '',
      usdtTxtNo: '',
      resimUrl: '',
      aciklama: ''
    }
  }

  filterChanged(createdDateStart: string, createdDateEnd: string) {
    this.kasaHaraketleriFilter.createdDateStart = createdDateStart;
    this.kasaHaraketleriFilter.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getAll(pageNumber: number = 1) {

    this.kasaHaraketleriFilter.pageNumber = pageNumber;

    this.kasaHaraketleriService.getAllWithPaging(this.kasaHaraketleriFilter).subscribe((response) => {
      if (response.success) {
        this.kasaHaraketleriList = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    });
  }

  downloadExcel() {
    this.kasaHaraketleriService.exportToExcel(this.kasaHaraketleriFilter).subscribe((response) => {
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

  closeKasaHaraketleriSilModal() {
    if (this.kasaHaraketleriSilModalKapatButton && this.kasaHaraketleriSilModalKapatButton.nativeElement) {
      this.kasaHaraketleriSilModalKapatButton.nativeElement.click();
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
      this.kasaHaraketleriService.delete(this.selectedItemId).subscribe({
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
          this.closeKasaHaraketleriSilModal();
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeKasaHaraketleriSilModal();
        }
      });
    }
  }

  startEdit(itemId: string) {
    this.kasaHaraketleriService.getById(itemId).subscribe({
      next: (response) => {
        if (!response.success) {
          this.alertify.error(response.message);
        }
        else {
          if (this.kasaHaraketleriEkleModalButton && this.kasaHaraketleriEkleModalButton.nativeElement) {
            this.kasaHaraketleriCreateModel = {
              id: response.result.id,
              tedarikciId: response.result.tedarikciId,
              aciklama: response.result.aciklama,
              tutar: response.result.tutar,
              dovizKuru: response.result.dovizKuru,
              usdtMiktar: response.result.usdtMiktar,
              usdtAdres: response.result.usdtAdres,
              usdtTxtNo: response.result.usdtTxtNo,
              resimUrl: response.result.resimUrl,
              islemTipi: response.result.islemTipi,
              durum: response.result.durum
            };

            this.kasaHaraketleriEkleModalButton.nativeElement.click();
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

  closeKasaHaraketleriEkleModal() {
    if (this.kasaHaraketleriEkleModalKapatButton && this.kasaHaraketleriEkleModalKapatButton.nativeElement) {
      this.kasaHaraketleriEkleModalKapatButton.nativeElement.click();
    }
  }

  openKasaHaraketleriEkleModal() {
    this.createEmptyModel();
    this.kasaHaraketleriEkleModalButton.nativeElement.click();
  }

  submitCreateKasaHaraketleriForm(kasaHaraketleriForm: NgForm) {
    if (!kasaHaraketleriForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    this.kasaHaraketleriCreateModel.durum = Number(this.kasaHaraketleriCreateModel.durum);

    if (this.isCreate()) {
      this.kasaHaraketleriService.create(this.kasaHaraketleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKasaHaraketleriEkleModal();
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
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }
    else {
      this.kasaHaraketleriService.update(this.kasaHaraketleriCreateModel).subscribe({
        next: response => {
          if (response.success) {
            this.closeKasaHaraketleriEkleModal();
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
          this.isCreating = false;
        },
        complete: () => {
        }
      });
    }

  }

  closeKasaHaraketleriEkleModalKapat() {
    this.createEmptyModel();
  }

  private isCreate(): boolean {
    return this.kasaHaraketleriCreateModel.id == "00000000-0000-0000-0000-000000000000";
  }

  getDurumText(durum: number) {
    if (durum == Durum.Bekliyor) return "Bekliyor";
    if (durum == Durum.Onaylandi) return "Onaylandı";
    if (durum == Durum.Red) return "Red";

    return "";
  }

  kasaGirisCikisToplam(islemTipi: number | number[]): number {
    const tipiList = Array.isArray(islemTipi) ? islemTipi : [islemTipi];
    if (!tipiList || tipiList.length === 0) {
      return 0;
    }

    return this.kasaHaraketleriList
      .filter(item => tipiList.includes(item.islemTipi))
      .reduce((sum, item) => sum + item.tutar, 0);
  }

  openVirmanEkleModal() {
    this.createEmptyVirmanModel();
    this.virmanEkleModalButton.nativeElement.click();
  }

  closeVirmanEkleModalKapat() {
    if (this.virmanEkleModalKapatButton && this.virmanEkleModalKapatButton.nativeElement) {
      this.createEmptyVirmanModel();
      this.virmanEkleModalKapatButton.nativeElement.click();
    }
  }

  onFromVirmanIslemTipiChange() {
    this.kasaHaraketleriVirmanCreateModel.toVirmanIslemTipi = this.kasaHaraketleriVirmanCreateModel.fromVirmanIslemTipi == 1 ? -1 : 1;
  }

  onToVirmanIslemTipiChange() {
    this.kasaHaraketleriVirmanCreateModel.fromVirmanIslemTipi = this.kasaHaraketleriVirmanCreateModel.toVirmanIslemTipi == 1 ? -1 : 1;
  }

  submitCreateKasaHaraketleriVirmanForm(kasaHaraketleriVirmanForm: NgForm) {
    if (!kasaHaraketleriVirmanForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;


    this.kasaHaraketleriVirmanService.create(this.kasaHaraketleriVirmanCreateModel).subscribe({
      next: response => {
        if (response.success) {
          this.closeVirmanEkleModalKapat();
          this.getAll();
          this.alertify.success("Kayıt eklendi");
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
