import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';
import { Authorization } from '../_constants/authorizations';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteMutabakatlariListDto } from '../_models/webSiteMutabakat/webSiteMutabakatList.dto';
import { WebSiteMutabakatFilter } from '../_models/webSiteMutabakat/webSiteMutabakatFilter';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { WebSiteMutabakatService } from '../_services/webSiteMutabakat.service';
import { WebsiteMutabakatCalculateDto } from '../_models/webSiteMutabakat/webSiteMutabakatCalculate.dto';

declare var $: any;

@Component({
  selector: 'app-site-mutabakat',
  templateUrl: './site-mutabakat.component.html',
  styleUrls: ['./site-mutabakat.component.css']
})
export class SiteMutabakatComponent {

  Authorization = Authorization;
  webSiteDropDownModel: WebSiteleriListDto[] = [];

  webSiteMutabakatlari: WebSiteMutabakatlariListDto[] = [];

  webSiteMutabakatFilter!: WebSiteMutabakatFilter;
  isCalculating: { [key: string]: boolean } = {};
  selectedItem: WebSiteMutabakatlariListDto | null = null;
  selectedIndex: number = -1;

  constructor(
    private webSiteMutabakatService: WebSiteMutabakatService,
    private webSiteService: WebSiteleriService,
    private alertify: AlertifyService
  ) {

    this.webSiteMutabakatFilter = {
      webSiteId: '',
      mutabakatTarihi: ''
    }

  }

  ngOnInit(): void {
    this.getWebSiteleri();

    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const formattedDate = yesterday.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' }).replace(/\./g, '/');
    this.webSiteMutabakatFilter.mutabakatTarihi = formattedDate;

    this.getAll();
  }

  ngAfterViewInit() {
    // Set endDate to yesterday to prevent selecting today and future dates
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);

    $('.dateControl').datepicker({
      language: 'tr',
      keyboardNavigation: false,
      forceParse: false,
      calendarWeeks: true,
      autoclose: true,
      format: "dd/mm/yyyy",
      endDate: yesterday
    });

  }

  filterChanged(mutabakatTarihi: string = '') {
    this.webSiteMutabakatFilter.mutabakatTarihi = mutabakatTarihi;
    this.getAll();
  }


  getWebSiteleri() {
    this.webSiteService.getSelectList().subscribe((reponse) => {
      this.webSiteDropDownModel = reponse.result;
    });
  }

  getAll() {
    this.webSiteMutabakatService.getAll(this.webSiteMutabakatFilter).subscribe((response) => {
      if (response.success) {
        this.webSiteMutabakatlari = response.result;
      }
      else {
        this.alertify.error(response.message);
      }
    });
  }

    calculate(item: WebSiteMutabakatlariListDto) {
      if (!item.webSiteId) {
        this.alertify.error('WebSite ID bulunamadı.');
        return;
      }
  
      // Create unique key for this item
      const key = `${item.webSiteId}_${this.webSiteMutabakatFilter.mutabakatTarihi}`;
  
      const calculateModel: WebsiteMutabakatCalculateDto = {
        webSiteId: item.webSiteId,
        devir: item.devir,
        mutabakatTarihi: this.webSiteMutabakatFilter.mutabakatTarihi
      };
  
      this.isCalculating[key] = true;
      this.webSiteMutabakatService.calculate(calculateModel).subscribe({
        next: (response) => {
          this.isCalculating[key] = false;
          if (response.success) {
            this.alertify.success('Mutabakat hesaplaması başarılı.');
            this.getAll();
          }
          else {
            this.alertify.error(response.message);
          }
        },
        error: (error) => {
          this.isCalculating[key] = false;
          this.alertify.error('Mutabakat hesaplaması sırasında bir hata oluştu.');
        }
      });
    }

      openDetailsModal(item: WebSiteMutabakatlariListDto, index: number) {
        this.selectedItem = item;
        this.selectedIndex = index;
        $('#detailsModal').modal('show');
      }

        copyToClipboard() {
    if (!this.selectedItem) return;

    const details = `

* ${this.selectedItem.webSiteAdi} MUTABAKAT *
${this.formatDate(this.selectedItem.mutabakatTarihi)}

Devir: ${this.formatCurrency(this.selectedItem.devir)}
Toplam Yatırım: ${this.formatCurrency(this.selectedItem.toplamYatirim)}
Toplam Yatırım Komisyon: ${this.formatCurrency(this.selectedItem.toplamYatirimKomisyon)}
Toplam Çekim: ${this.formatCurrency(this.selectedItem.toplamCekim)}
Toplam Çekim Komisyon: ${this.formatCurrency(this.selectedItem.toplamCekimKomisyon)}
Alınan Ödeme: ${this.formatCurrency(this.selectedItem.toplamGiris)}
Yapılan Ödeme: ${this.formatCurrency(this.selectedItem.toplamCikis)}
Yapılan Ödeme Komisyonu: ${this.formatCurrency(this.selectedItem.toplamCikisKomisyon)}
Manuel Alınan Ödeme: ${this.formatCurrency(this.selectedItem.toplamGirisManual)}
Manuel Yapılan Ödeme: ${this.formatCurrency(this.selectedItem.toplamCikisManual)}
Virman (+): ${this.formatCurrency(this.selectedItem.toplamVirmanGiris)}
Virman (-): ${this.formatCurrency(this.selectedItem.toplamVirmanCikis)}
Kasa: ${this.formatCurrency(this.selectedItem.kasa)}

    `;

    navigator.clipboard.writeText(details.trim()).then(() => {
      this.alertify.success('Detaylar panoya kopyalandı.');
    }).catch(err => {
      this.alertify.error('Kopyalama başarısız.');
    });
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);
  }

  private formatDate(date: any): string {
    const d = new Date(date);
    return d.toLocaleDateString('tr-TR');
  }


}
