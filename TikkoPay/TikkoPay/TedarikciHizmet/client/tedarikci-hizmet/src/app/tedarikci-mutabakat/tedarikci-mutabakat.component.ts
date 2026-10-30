import { Component, OnInit } from '@angular/core';
import { Authorization } from '../_constants/authorizations';
import { TedarikciMutabakatFilter } from '../_models/tedarikciMutabakat/tedarikciMutabakatFilter';
import { TedarikciMutabakatlariListDto } from '../_models/tedarikciMutabakat/tedarikciMutabakatList.dto';
import { AlertifyService } from '../_services/alertify.service';
import { TedarikciMutabakatService } from '../_services/tedarikciMutabakat.service';

declare var $: any;

@Component({
  selector: 'app-tedarikci-mutabakat',
  templateUrl: './tedarikci-mutabakat.component.html',
  styleUrls: ['./tedarikci-mutabakat.component.css']
})
export class TedarikciMutabakatComponent implements OnInit {

  Authorization = Authorization;

  tedarikciMutabakatlari: TedarikciMutabakatlariListDto[] = [];

  tedarikciMutabakatFilter!: TedarikciMutabakatFilter;
  isCalculating: { [key: string]: boolean } = {};
  selectedItem: TedarikciMutabakatlariListDto | null = null;
  selectedIndex: number = -1;

  constructor(
    private tedarikciMutabakatService: TedarikciMutabakatService,
    private alertify: AlertifyService
  ) {
    this.tedarikciMutabakatFilter = {
      mutabakatTarihi: ''
    }
  }

  ngOnInit(): void {

    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const formattedDate = yesterday.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' }).replace(/\./g, '/');
    this.tedarikciMutabakatFilter.mutabakatTarihi = formattedDate;

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
    this.tedarikciMutabakatFilter.mutabakatTarihi = mutabakatTarihi;
    this.getAll();
  }

  getAll() {
    this.tedarikciMutabakatService.getAll(this.tedarikciMutabakatFilter).subscribe((response) => {
      if (response.success) {
        this.tedarikciMutabakatlari = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    });
  }

  openDetailsModal(item: TedarikciMutabakatlariListDto, index: number) {
    this.selectedItem = item;
    this.selectedIndex = index;
    $('#detailsModal').modal('show');
  }

  copyToClipboard() {
    if (!this.selectedItem) return;

    const details = `

* ${this.selectedItem.tedarikciAdi} MUTABAKAT *
${this.formatDate(this.selectedItem.mutabakatTarihi)}

Devir: ${this.formatCurrency(this.selectedItem.devir)}
Toplam Yatırım: ${this.formatCurrency(this.selectedItem.toplamYatirim)}
Toplam Yatırım Komisyon: ${this.formatCurrency(this.selectedItem.toplamYatirimKomisyon)}
Toplam Çekim: ${this.formatCurrency(this.selectedItem.toplamCekim)}
Toplam Çekim Komisyon: ${this.formatCurrency(this.selectedItem.toplamCekimKomisyon)}
Alınan Ödeme: ${this.formatCurrency(this.selectedItem.toplamGiris)}
Ödeme Komisyonu: ${this.formatCurrency(this.selectedItem.toplamGirisKomisyon)}
Yapılan Ödeme: ${this.formatCurrency(this.selectedItem.toplamCikis)}
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
