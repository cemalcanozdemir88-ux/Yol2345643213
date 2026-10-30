import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Authorization } from '../_constants/authorizations';
import { RecordsPerPage } from '../_constants/constants';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { BankaService } from '../_services/banka.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { ManuelYatirimTalepleriService } from '../_services/manuelYatirimTalepleri.service';

declare var $: any;

@Component({
  selector: 'app-manual-bekleyen-yatirimlar',
  templateUrl: './manual-bekleyen-yatirimlar.component.html',
  styleUrls: ['./manual-bekleyen-yatirimlar.component.css']
})
export class ManualBekleyenYatirimlarComponent implements OnInit {
  
  Authorization = Authorization;

  @ViewChild('updatePriceModalButton') updatePriceModalButton!: ElementRef;
  @ViewChild('updatePriceModalKapatButton') updatePriceModalKapatButton!: ElementRef;

  @ViewChild('iptalModalKapatButton') iptalModalKapatButton!: ElementRef;
  @ViewChild('onayModalKapatButton') onayModalKapatButton!: ElementRef;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];
  
  filterModel!: YatirimTalepleriPagingFilter;

  selectedItemId: string = '';

  activePage: number = 1;
  totalPages: number = 0;

  public isUpdating: boolean = false;

  constructor(
    private authService: AuthService,
    private yatirimTalepleriService: ManuelYatirimTalepleriService,
    private alertify: AlertifyService) {

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: this.authService.getWebsiteId(),
      searchText: '',
      approveDateStart: '',
      approveDateEnd: '',
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: 999999
    }

  }

  ngOnInit(): void {

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
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
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

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }

  formatDateToSlash(date: Date): string {
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\./g, '/');
  }
}
