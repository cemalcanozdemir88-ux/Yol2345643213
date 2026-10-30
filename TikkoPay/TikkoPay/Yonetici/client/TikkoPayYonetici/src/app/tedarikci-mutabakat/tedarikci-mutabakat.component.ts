import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TedarikciMutabakatService } from '../_services/tedarikciMutabakat.service';
import { AlertifyService } from '../_services/alertify.service';
import { TedarikciMutabakatlariListDto } from '../_models/tedarikciMutabakat/tedarikciMutabakatList.dto';
import { TedarikciMutabakatFilter } from '../_models/tedarikciMutabakat/tedarikciMutabakatFilter';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciService } from '../_services/tedarikci.service';
import { Authorization } from '../_constants/authorizations';
import { TedarikciMutabakatCalculateDto } from '../_models/tedarikciMutabakat/tedarikciMutabakatCalculate.dto';

declare var $: any;

@Component({
  selector: 'app-tedarikci-mutabakat',
  templateUrl: './tedarikci-mutabakat.component.html',
  styleUrls: ['./tedarikci-mutabakat.component.css']
})
export class TedarikciMutabakatComponent implements OnInit {

  Authorization = Authorization;

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  tedarikciMutabakatlari: TedarikciMutabakatlariListDto[] = [];

  tedarikciMutabakatFilter!: TedarikciMutabakatFilter;
  isCalculating: { [key: string]: boolean } = {};
  selectedItem: TedarikciMutabakatlariListDto | null = null;
  selectedIndex: number = -1;

  constructor(
    private tedarikciMutabakatService: TedarikciMutabakatService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService
  ) {
    this.tedarikciMutabakatFilter = {
      tedarikciId: '',
      mutabakatTarihi: ''
    }
  }

  ngOnInit(): void {
    this.getTedarikciler();

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

  getTedarikciler() {
    this.tedarikcilerService.getSelectList().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
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

  calculate(item: TedarikciMutabakatlariListDto) {
    if (!confirm('Mutabakat hesaplaması yapmak istediğinizden emin misiniz?')) {
      return;
    }

    if (!item.tedarikciId) {
      this.alertify.error('Tedarikçi ID bulunamadı.');
      return;
    }

    // Create unique key for this item
    const key = `${item.tedarikciId}_${this.tedarikciMutabakatFilter.mutabakatTarihi}`;

    const calculateModel: TedarikciMutabakatCalculateDto = {
      tedarikciId: item.tedarikciId,
      devir: item.devir,
      mutabakatTarihi: this.tedarikciMutabakatFilter.mutabakatTarihi
    };

    this.isCalculating[key] = true;
    this.tedarikciMutabakatService.calculate(calculateModel).subscribe({
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
