import { Component, ElementRef, ViewChild } from '@angular/core';
import { Authorization } from '../_constants/authorizations';
import { RecordsPerPage } from '../_constants/constants';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { ManuelYatirimTalepleriService } from '../_services/manuelYatirimTalepleri.service';
import { NgForm } from '@angular/forms';

declare var $: any;

@Component({
  selector: 'app-manual-iptal-yatirimlar',
  templateUrl: './manual-iptal-yatirimlar.component.html',
  styleUrls: ['./manual-iptal-yatirimlar.component.css']
})
export class ManualIptalYatirimlarComponent {

  Authorization = Authorization;

  @ViewChild('bekleyeneAlModalKapatButton') bekleyeneAlModalKapatButton!: ElementRef;

  public isLoading: boolean = false;

  yatirimTalepleri: YatirimTalepleriListDto[] = [];

  filterModel!: YatirimTalepleriPagingFilter;

  selectedItemId: string = "";

  activePage: number = 1;
  totalPages: number = 0;

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
      recordsPerPage: RecordsPerPage
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

  filterChanged(approveDateStart: string, approveDateEnd: string, createdDateStart: string, createdDateEnd: string) {
    this.filterModel.approveDateStart = approveDateStart;
    this.filterModel.approveDateEnd = approveDateEnd;
    this.filterModel.createdDateStart = createdDateStart;
    this.filterModel.createdDateEnd = createdDateEnd;

    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.yatirimTalepleriService.getRedYatirimTalepleriWithPaging(this.filterModel).subscribe((response) => {
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

    startBekleyeneAl(itemId: string) {
    this.selectedItemId = itemId;
  }

    closeBekleyeneAlModalKapat() {
    this.selectedItemId = "";
    this.bekleyeneAlModalKapatButton.nativeElement.click();
  }

    submitBekleyeneAlForm(bekleyeneAlForm: NgForm){
      if (!this.selectedItemId) {
        this.alertify.error("Yatırım seçiniz!");
        return;
      }
  
      this.isLoading = true;
  
      this.yatirimTalepleriService.bekleyeneAl(this.selectedItemId).subscribe({
        next: response => {
          if (response.success) {
            this.closeBekleyeneAlModalKapat();
            this.getAll();
            this.alertify.success(response.message);
          }
          else {
            this.alertify.error(response.message);
          }
          this.isLoading = false;
        },
        error: error => {
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        }
      });
  
    }

}


