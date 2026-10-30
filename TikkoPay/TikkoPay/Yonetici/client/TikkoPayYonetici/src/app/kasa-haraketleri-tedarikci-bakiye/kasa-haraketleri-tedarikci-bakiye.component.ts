import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciBakiyeListDto } from '../_models/tedarikci/tedarikciBakiyetList.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { AlertifyService } from '../_services/alertify.service';
import { NgForm } from '@angular/forms';
import { KasaHaraketleriCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriCreate.dto';
import { KasaHaraketleriService } from '../_services/kasaharaketleri.service';
import { NgxCurrencyDirective } from 'ngx-currency';
import { Authorization } from '../_constants/authorizations';

@Component({
  selector: 'app-kasa-haraketleri-tedarikci-bakiye',
  templateUrl: './kasa-haraketleri-tedarikci-bakiye.component.html',
  styleUrls: ['./kasa-haraketleri-tedarikci-bakiye.component.css']
})
export class KasaHaraketleriTedarikciBakiyeComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kasaHaraketleriEkleModalButton') kasaHaraketleriEkleModalButton!: ElementRef;
  @ViewChild('kasaHaraketleriEkleModalKapatButton') kasaHaraketleriEkleModalKapatButton!: ElementRef;

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  tedarikciBakiyeList: TedarikciBakiyeListDto[] = [];

  kasaHaraketleriCreateModel!: KasaHaraketleriCreateDto;

  selectedTedarikci: string = "";
  tedarikciDurum: number = 1;

  public isCreating: boolean = false;

  constructor(
    private kasaHaraketleriService: KasaHaraketleriService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService) {

    this.createEmptyModel();
  }

  ngOnInit(): void {
    this.getTedarikciler();
    this.getAll();
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

  filterChanged() {
    this.getAll();
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getAll() {
    this.tedarikcilerService.getBakiyeList(this.selectedTedarikci, this.tedarikciDurum).subscribe((response) => {
      if (response.success) {
        this.tedarikciBakiyeList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }
    })
  }

  startEdit(itemId: string, islemTipi: string) {
    this.kasaHaraketleriCreateModel.tedarikciId = itemId;
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

  getAnlikBakiye(item: TedarikciBakiyeListDto): number {
    var yatirim = (item.toplamYatirim - item.toplamYatirimKomisyon);
    var cekim = (item.toplamCekim + item.toplamCekimKomisyon);
    var kasaHaraketGiris = (1 * item.toplamKasaHaraketiGiris);
    var kasaHaraketCikis = (1 * item.toplamKasaHaraketiCikis);

    var val1 = yatirim + kasaHaraketCikis;
    var val2 = -1 * (cekim + kasaHaraketGiris);

    return val1 + val2;
  }

}
