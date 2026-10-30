import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { Authorization } from '../_constants/authorizations';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
  ]
})
export class HomeComponent implements OnInit {

  Authorization = Authorization;

  yatirimFilterModel!: YatirimTalepleriPagingFilter;
  cekimFilterModel!: CekimTalepleriPagingFilter;

  yatirimToplamTutar: number = 0;
  yatirimTedarikciToplamKomisyon: number = 0;
  yatirimWebsiteToplamKomisyon: number = 0;
  yatirimToplamKazanc: number = 0;

  yatirimToplamTutarYesterday: number = 0;
  yatirimTedarikciToplamKomisyonYesterday: number = 0;
  yatirimWebsiteToplamKomisyonYesterday: number = 0;
  yatirimToplamKazancYesterday: number = 0;

  yatirimToplamTutarMonth: number = 0;
  yatirimTedarikciToplamKomisyonMonth: number = 0;
  yatirimWebsiteToplamKomisyonMonth: number = 0;
  yatirimToplamKazancMonth: number = 0;

  cekimToplamTutar: number = 0;
  cekimTedarikciToplamKomisyon: number = 0;
  cekimWebsiteToplamKomisyon: number = 0;
  cekimToplamKazanc: number = 0;

  cekimToplamTutarYesterday: number = 0;
  cekimTedarikciToplamKomisyonYesterday: number = 0;
  cekimWebsiteToplamKomisyonYesterday: number = 0;
  cekimToplamKazancYesterday: number = 0;

  cekimToplamTutarMonth: number = 0;
  cekimTedarikciToplamKomisyonMonth: number = 0;
  cekimWebsiteToplamKomisyonMonth: number = 0;
  cekimToplamKazancMonth: number = 0;


  constructor(
    private authService: AuthService,
    private yatirimTalepleriService: YatirimTalepleriService,
    private cekimTalepleriService: CekimTalepleriService,
    private router: Router) {

      this.yatirimFilterModel = {
        tedarikciId: '',
        bankaId: '',
        webSiteId: '',
        searchText: '',
        approveDateStart: '',
        approveDateEnd: '',
        createdDateStart: '',
        createdDateEnd: '', 
        pageNumber: 1,
        recordsPerPage: 20
      }

      this.cekimFilterModel = {
        tedarikciId: '',
        bankaId: '',
        webSiteId: '',
        searchText: '',
        approveDateStart: '',
        approveDateEnd: '',
        createdDateStart: '',
        createdDateEnd: '', 
        pageNumber: 1,
        recordsPerPage: 20
      }

  }

  ngOnInit(): void {
    
    if (this.authService.isTedarikci()) {
      this.router.navigate(['/tedarikci-bekleyen-yatirimlar']);
    }
    else if (this.authService.isWebSiteKullanicisi()) {
      this.router.navigate(['/website-bakiye']);
    }
    else if (this.authService.isAffiliateKullanicisi()) {
      this.router.navigate(['/affiliate-bakiye']);
    }
    
    if (this.isAdmin() && this.authService.userHasAuthorization(Authorization.AnaSayfaHesapOzetiGorebilir)) {
      this.getToplamYatirim();
      this.getToplamYatirimYesterday();
      this.getToplamYatirimMonth();

      this.getToplamCekim();
      this.getToplamCekimYesterday();
      this.getToplamCekimMonth();
    }

  }

  getToplamYatirim() {

    const todayString = this.getToday();
    this.yatirimFilterModel.createdDateStart = todayString;
    this.yatirimFilterModel.createdDateEnd = todayString;

    this.yatirimTalepleriService.getToplamYatirimTutari(this.yatirimFilterModel).subscribe((response) => {
      this.yatirimToplamTutar = response.result.toplamTutar;
      this.yatirimTedarikciToplamKomisyon = response.result.tedarikciToplamKomisyon;
      this.yatirimWebsiteToplamKomisyon = response.result.websiteToplamKomisyon;

      this.yatirimToplamKazanc = this.yatirimWebsiteToplamKomisyon - this.yatirimTedarikciToplamKomisyon;

    });
  }

  getToplamYatirimYesterday() {
    
    const yesterdayString = this.getYesterday();
    this.yatirimFilterModel.createdDateStart = yesterdayString;
    this.yatirimFilterModel.createdDateEnd = yesterdayString;

    this.yatirimTalepleriService.getToplamYatirimTutari(this.yatirimFilterModel).subscribe((response) => {
      this.yatirimToplamTutarYesterday = response.result.toplamTutar;
      this.yatirimTedarikciToplamKomisyonYesterday = response.result.tedarikciToplamKomisyon;
      this.yatirimWebsiteToplamKomisyonYesterday = response.result.websiteToplamKomisyon;

      this.yatirimToplamKazancYesterday = this.yatirimWebsiteToplamKomisyonYesterday - this.yatirimTedarikciToplamKomisyonYesterday;

    });
  }

  getToplamYatirimMonth() {
    this.yatirimFilterModel.createdDateStart = this.getFirstDayOfMonth();
    this.yatirimFilterModel.createdDateEnd = this.getLastDayOfMonth();

    this.yatirimTalepleriService.getToplamYatirimTutari(this.yatirimFilterModel).subscribe((response) => {
      this.yatirimToplamTutarMonth = response.result.toplamTutar;
      this.yatirimTedarikciToplamKomisyonMonth = response.result.tedarikciToplamKomisyon;
      this.yatirimWebsiteToplamKomisyonMonth = response.result.websiteToplamKomisyon;

      this.yatirimToplamKazancMonth = this.yatirimWebsiteToplamKomisyonMonth - this.yatirimTedarikciToplamKomisyonMonth;

    });
  }

  getToplamCekim() {

    const todayString = this.getToday();
    this.cekimFilterModel.createdDateStart = todayString;
    this.cekimFilterModel.createdDateEnd = todayString;

    this.cekimTalepleriService.getToplamCekimTutari(this.cekimFilterModel).subscribe((response) => {
      this.cekimToplamTutar = response.result.toplamTutar;
      this.cekimTedarikciToplamKomisyon = response.result.tedarikciToplamKomisyon;
      this.cekimWebsiteToplamKomisyon = response.result.websiteToplamKomisyon;

      this.cekimToplamKazanc = this.cekimWebsiteToplamKomisyon - this.cekimTedarikciToplamKomisyon;

    });
  }

  getToplamCekimYesterday() {

    const yesterdayString = this.getYesterday();
    this.cekimFilterModel.createdDateStart = yesterdayString;
    this.cekimFilterModel.createdDateEnd = yesterdayString;

    this.cekimTalepleriService.getToplamCekimTutari(this.cekimFilterModel).subscribe((response) => {
      this.cekimToplamTutarYesterday = response.result.toplamTutar;
      this.cekimTedarikciToplamKomisyonYesterday = response.result.tedarikciToplamKomisyon;
      this.cekimWebsiteToplamKomisyonYesterday = response.result.websiteToplamKomisyon;

      this.cekimToplamKazancYesterday = this.cekimWebsiteToplamKomisyonYesterday - this.cekimTedarikciToplamKomisyonYesterday;

    });
  }

  getToplamCekimMonth() {

    this.cekimFilterModel.createdDateStart = this.getFirstDayOfMonth();
    this.cekimFilterModel.createdDateEnd = this.getLastDayOfMonth();

    this.cekimTalepleriService.getToplamCekimTutari(this.cekimFilterModel).subscribe((response) => {
      this.cekimToplamTutarMonth = response.result.toplamTutar;
      this.cekimTedarikciToplamKomisyonMonth = response.result.tedarikciToplamKomisyon;
      this.cekimWebsiteToplamKomisyonMonth = response.result.websiteToplamKomisyon;

      this.cekimToplamKazancMonth = this.cekimWebsiteToplamKomisyonMonth - this.cekimTedarikciToplamKomisyonMonth;

    });
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  getToday(): string {
    const today = new Date();
    const day = today.getDate().toString().padStart(2, '0'); 
    const month = (today.getMonth() + 1).toString().padStart(2, '0'); 
    const year = today.getFullYear().toString(); 

    const todayString = `${day}/${month}/${year}`;

    return todayString;
  }

  getYesterday(): string {
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
  
    const day = yesterday.getDate().toString().padStart(2, '0'); 
    const month = (yesterday.getMonth() + 1).toString().padStart(2, '0'); 
    const year = yesterday.getFullYear().toString(); 
  
    const yesterdayString = `${day}/${month}/${year}`;
  
    return yesterdayString;
  }

  getFirstDayOfMonth(): string {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
  
    const day = firstDay.getDate().toString().padStart(2, '0'); 
    const month = (firstDay.getMonth() + 1).toString().padStart(2, '0'); 
    const year = firstDay.getFullYear().toString();
  
    return `${day}/${month}/${year}`;
  }
  
  getLastDayOfMonth(): string {
    const today = new Date();
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
  
    const day = lastDay.getDate().toString().padStart(2, '0'); 
    const month = (lastDay.getMonth() + 1).toString().padStart(2, '0'); 
    const year = lastDay.getFullYear().toString();
  
    return `${day}/${month}/${year}`;
  }
}

