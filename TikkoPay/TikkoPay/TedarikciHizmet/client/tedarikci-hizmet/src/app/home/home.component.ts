import { Component, OnInit } from '@angular/core';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { KasaHaraketleriService } from '../_services/kasaharaketleri.service';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
  ]
})
export class HomeComponent implements OnInit {

  toplamKasaHaraketiGiris: number = 0;
  toplamKasaHaraketiCikis: number = 0;

  toplamYatirimTutari: number = 0;
  toplamYatirimTutariKomisyon: number = 0;

  toplamCekimTutari: number = 0;
  toplamCekimTutariKomisyon: number = 0;

  anlikBakiye: number = 0;

  constructor(
    private yatirimTalepleriService: YatirimTalepleriService,
    private cekimTalepleriService: CekimTalepleriService,
    private kasaHaraketleriService: KasaHaraketleriService) {

  }

  ngOnInit(): void {
    forkJoin({
      kasaHaraketGiris: this.kasaHaraketleriService.getToplamKasaHaraketiGiris(),
      kasaHaraketCikis: this.kasaHaraketleriService.getToplamKasaHaraketiCikis(),
      yatirimTutari: this.yatirimTalepleriService.getToplamYatirimTutari(),
      cekimTutari: this.cekimTalepleriService.getToplamCekimTutari()
  }).subscribe({
      next: (results) => {
          this.toplamKasaHaraketiGiris = results.kasaHaraketGiris.result.sum;
          this.toplamKasaHaraketiCikis = results.kasaHaraketCikis.result.sum;
          this.toplamYatirimTutari = results.yatirimTutari.result.toplamTutar;
          this.toplamYatirimTutariKomisyon = results.yatirimTutari.result.toplamKomisyon;
          this.toplamCekimTutari = results.cekimTutari.result.toplamTutar;
          this.toplamCekimTutariKomisyon = results.cekimTutari.result.toplamKomisyon;

          this.anlikBakiye = this.getAnlikBakiye();
      },
      error: (err) => {
      }
  });
  }

  ngAfterViewInit() {
    
  }

  getToplamKasaHaraketiGiris(){
    this.kasaHaraketleriService.getToplamKasaHaraketiGiris().subscribe((response) => {
      this.toplamKasaHaraketiGiris = response.result.sum;
    })
  }

  getToplamKasaHaraketiCikis(){
    this.kasaHaraketleriService.getToplamKasaHaraketiCikis().subscribe((response) => {
      this.toplamKasaHaraketiCikis = response.result.sum;
    })
  }

  getToplamYatirimTutari(){
    this.yatirimTalepleriService.getToplamYatirimTutari().subscribe((response) => {
      this.toplamYatirimTutari = response.result.toplamTutar;
      this.toplamYatirimTutariKomisyon = response.result.toplamKomisyon;
    })
  }

  getToplamCekimTutari(){
    this.cekimTalepleriService.getToplamCekimTutari().subscribe((response) => {
      this.toplamCekimTutari = response.result.toplamTutar;
      this.toplamCekimTutariKomisyon = response.result.toplamKomisyon;
    })
  }

  getAnlikBakiye(){
    var yatirim = this.toplamYatirimTutari - this.toplamYatirimTutariKomisyon;
    var cekim = (this.toplamCekimTutari + this.toplamCekimTutariKomisyon);
    var kasaHaraketGiris = (1 * this.toplamKasaHaraketiGiris);
    var kasaHaraketCikis = (1 * this.toplamKasaHaraketiCikis);

    var val1 = yatirim + kasaHaraketCikis;
    var val2 = -1 * (cekim + kasaHaraketGiris);

    return val1 + val2;
  }

}

