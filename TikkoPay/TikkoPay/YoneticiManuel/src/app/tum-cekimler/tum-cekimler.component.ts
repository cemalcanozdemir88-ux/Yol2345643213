import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { BankaService } from '../_services/banka.service';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTaleperliList.dto';
import { GetDekontDto } from '../_models/cekimTalepleri/getDekont.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { TumCekimTalepleriFilter } from '../_models/cekimTalepleri/tumCekimTalepleriFilter';
import { YatirimCekimTalepleriDurum } from '../_models/yatirimTalepleri/yatirimCekimTalepleriDurum.Enum';
import { Authorization } from '../_constants/authorizations';

declare var $: any;

@Component({
  selector: 'app-tum-cekimler',
  templateUrl: './tum-cekimler.component.html',
  styleUrls: ['./tum-cekimler.component.css']
})
export class TumCekimlerComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('cokluTedarikciModal') cokluTedarikciModal!: ElementRef;
  @ViewChild('dekontModal') dekontModal!: ElementRef;
  @ViewChild('dekontModalKapatButton') dekontModalKapatButton!: ElementRef;

  public isLoading: boolean = false;

  cekimTalepleri: CekimTalepleriListDto[] = [];
  bankalarDropDownModel: BankaListDto[] = [];
  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];
  webSiteleriDropDownModel: WebSiteleriListDto[] = [];
  cekimTedarikciAtamalari: CekimTalepleriListDto[] = [];
  getDekontDto: GetDekontDto;

  filterModel!: TumCekimTalepleriFilter;

  activePage: number = 1;
  totalPages: number = 0;

  selectedItemId: string = "";
  private cdr: ChangeDetectorRef;

  constructor(
    cdr: ChangeDetectorRef,
    private authService: AuthService,
    private cekimTalepleriService: CekimTalepleriService,
    private bankaService: BankaService,
    private tedarikcilerService: TedarikciService,
    private webSiteleriService: WebSiteleriService,
    private alertify: AlertifyService
  ) {

    this.cdr = cdr;

    const today = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(today.getMonth() - 1);

    const formatDate = (date: Date) => {
      return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
    }

    this.getDekontDto = {
      id: '',
      dekontKontrolDurumu: 0,
      dekontKontrolKullanici: '',
      dekontKontrolTarihi: '',
      info: ''
    }

    this.filterModel = {
      tedarikciId: '',
      bankaId: '',
      webSiteId: this.isWebSiteKullanicisi() ? this.authService.getWebsiteId() : '',
      durum: '',
      searchText: '',
      dekontKontrolDurum: '',
      createdDateStart: formatDate(oneMonthAgo),
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: 40
    }

  }


  ngOnInit(): void {
    if (this.isAdmin()) {
      this.getBankalar();
      this.getTedarikciler();
    }

    this.getWebSiteleri();
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

  getBankalar() {
    this.bankaService.getAll().subscribe((reponse) => {
      this.bankalarDropDownModel = reponse.result;
    });
  }

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getWebSiteleri() {
    this.webSiteleriService.getSelectList().subscribe((response) => {
      this.webSiteleriDropDownModel = response.result;
    })
  }

  getAll(pageNumber: number = 1) {

    this.filterModel.pageNumber = pageNumber;

    this.cekimTalepleriService.getTumCekimTalepleriWithPaging(this.filterModel).subscribe((response) => {
      if (response.success) {
        this.cekimTalepleri = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isWebSiteKullanicisi() {
    return this.authService.isWebSiteKullanicisi();
  }

  cancelDelete() {
    this.selectedItemId = "";
  }

  getDurumText(durum: number) {
    if (durum == YatirimCekimTalepleriDurum.Bekliyor) return "Bekliyor";
    if (durum == YatirimCekimTalepleriDurum.Onaylandi) return "Onaylandı";
    if (durum == YatirimCekimTalepleriDurum.Red) return "Red";

    return "";
  }

  formatDateToSlash(date: Date): string {
    return date.toLocaleDateString('tr-TR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).replace(/\./g, '/');
  }

  startCokluTedarikciModal(item: CekimTalepleriListDto) {

    this.isLoading = true;

    this.cekimTalepleriService.getByTopId(item.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.cekimTedarikciAtamalari = response.result;

          this.isLoading = false;

          this.cdr.detectChanges();
          $(this.cokluTedarikciModal.nativeElement).modal('show');

        } else {
          this.isLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.alertify.error('Çekim tedarikçi atama bilgileri alınırken bir hata oluştu.');
      }
    });
  }

  startDekontModal(item: CekimTalepleriListDto) {
    this.isLoading = true;

    this.getDekontDto = {
      id: '',
      dekontKontrolDurumu: 0,
      dekontKontrolKullanici: '',
      dekontKontrolTarihi: '',
      info: ''
    }

    this.cekimTalepleriService.getDekont(item.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.getDekontDto = response.result;

          this.isLoading = false;

          this.cdr.detectChanges();
          $(this.dekontModal.nativeElement).modal('show');

        } else {
          this.isLoading = false;
          this.alertify.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.alertify.error('Dekont bilgileri alınırken bir hata oluştu.');
      }
    });

  }

}
