import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Authorization } from '../_constants/authorizations';
import { BakiyeListDto } from '../_models/kasaHaraketleri/bakiyeList.dto';
import { KasaHaraketleriCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriCreate.dto';
import { AlertifyService } from '../_services/alertify.service';
import { KasaHaraketleriService } from '../_services/kasaharaketleri.service';

@Component({
  selector: 'app-kasa-haraketleri-bakiye',
  templateUrl: './kasa-haraketleri-bakiye.component.html',
  styleUrls: []
})
export class KasaHaraketleriBakiyeComponent implements OnInit{

  Authorization = Authorization;

  @ViewChild('kasaHaraketleriEkleModalButton') kasaHaraketleriEkleModalButton!: ElementRef;
  @ViewChild('kasaHaraketleriEkleModalKapatButton') kasaHaraketleriEkleModalKapatButton!: ElementRef;

  bakiyeList: BakiyeListDto[] = [];

  kasaHaraketleriCreateModel!: KasaHaraketleriCreateDto;

  public isCreating: boolean = false;

  constructor(
    private kasaHaraketleriService: KasaHaraketleriService,
    private alertify: AlertifyService) {

    this.createEmptyModel();
  }

  ngOnInit(): void {
    this.getAll();
  }

  createEmptyModel() {
    this.kasaHaraketleriCreateModel = {
      id: '00000000-0000-0000-0000-000000000000',
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

  filterChanged() {
    this.getAll();
  }

  getAll() {
    this.kasaHaraketleriService.getBakiyeList().subscribe((response) => {
      if (response.success) {
        this.bakiyeList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }
    })
  }

  startEdit(itemId: string, islemTipi: string) {
    this.kasaHaraketleriCreateModel.islemTipi = Number(islemTipi);

    this.kasaHaraketleriEkleModalButton.nativeElement.click();
  }

  submitCreateKasaHaraketleriForm(kasaHaraketleriForm: NgForm) {
    if (!kasaHaraketleriForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    this.kasaHaraketleriCreateModel.durum = Number(this.kasaHaraketleriCreateModel.durum);

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
        this.alertify.error(error.error.message);
        this.isCreating = false;
      },
      complete: () => {
      }
    });
  }

  closeKasaHaraketleriEkleModal() {
    if (this.kasaHaraketleriEkleModalKapatButton && this.kasaHaraketleriEkleModalKapatButton.nativeElement) {
      this.kasaHaraketleriEkleModalKapatButton.nativeElement.click();
    }
  }

  closeKasaHaraketleriEkleModalKapat() {
    this.createEmptyModel();
  }

  getAnlikBakiye(item: BakiyeListDto): number {
    var yatirim = (item.toplamYatirim - item.toplamYatirimKomisyon);
    var cekim = (item.toplamCekim + item.toplamCekimKomisyon);
    var kasaHaraketGiris = (1 * item.toplamKasaHaraketiGiris);
    var kasaHaraketCikis = (1 * item.toplamKasaHaraketiCikis);

    var val1 = yatirim + kasaHaraketCikis;
    var val2 = -1 * (cekim + kasaHaraketGiris);

    return val1 + val2;
  }

}
