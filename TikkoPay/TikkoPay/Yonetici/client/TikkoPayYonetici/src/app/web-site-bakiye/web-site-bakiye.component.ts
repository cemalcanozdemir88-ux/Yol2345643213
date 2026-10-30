import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { KasaHaraketleriService } from '../_services/kasaharaketleri.service';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteBakiyeListDto } from '../_models/websiteleri/webSiteBakiyeList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';

@Component({
  selector: 'app-web-site-bakiye',
  templateUrl: './web-site-bakiye.component.html',
  styleUrls: ['./web-site-bakiye.component.css']
})
export class WebSiteBakiyeComponent implements OnInit {

  webSiteDropDownModel: WebSiteleriListDto[] = [];
  webSiteBakiyeList: WebSiteBakiyeListDto[] = [];

  selectedWebsite: string = "";

  constructor(
    private authService: AuthService,
    private kasaHaraketleriService: KasaHaraketleriService,
    private webSiteService: WebSiteleriService,
    private alertify: AlertifyService) {

      this.selectedWebsite = this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '';

  }

  ngOnInit(): void {
    this.getWebSiteleri();
    this.getAll();
  }

  filterChanged() {
    this.getAll();
  }

  getWebSiteleri() {
    this.webSiteService.getSelectList().subscribe((reponse) => {
      this.webSiteDropDownModel = reponse.result;
    });
  }

  getAll() {
    this.webSiteService.getBakiyeList(this.selectedWebsite).subscribe((response) => {
      if (response.success) {
        this.webSiteBakiyeList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }
    })
  }

  getAnlikBakiye(item: WebSiteBakiyeListDto): number {

    var yatirim = (item.toplamYatirim - item.toplamYatirimKomisyon);
    var cekim = (item.toplamCekim + item.toplamCekimKomisyon);

    return yatirim - cekim + item.toplamKasaHaraketiGiris - item.toplamKasaHaraketiCikis;

  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }
}
