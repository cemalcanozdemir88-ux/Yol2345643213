import { Component } from '@angular/core';
import { RecordsPerPage } from 'src/app/_constants/constants';
import { Durum } from 'src/app/_enums/durum.enum';
import { KasaHaraketleriPagingFilter } from 'src/app/_models/kasaHaraketleri/kasaHaraketleriFilter';
import { KasaHaraketleriListDto } from 'src/app/_models/kasaHaraketleri/kasaHaraketleriList.dto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { KasaHaraketleriService } from 'src/app/_services/kasaharaketleri.service';


@Component({
  selector: 'app-kasa-haraketleri-red',
  templateUrl: './kasa-haraketleri-red.component.html',
  styleUrls: ['./kasa-haraketleri-red.component.css']
})
export class KasaHaraketleriRedComponent {

  kasaHaraketleriList: KasaHaraketleriListDto[] = [];
  kasaHaraketleriFilter!: KasaHaraketleriPagingFilter;

  activePage: number = 1;
  totalPages: number = 0;

  constructor(
    private kasaHaraketleriService: KasaHaraketleriService,
    private authService: AuthService,
    private alertify: AlertifyService) {

    this.kasaHaraketleriFilter = {
      tedarikciId: this.authService.getTedarikciId(),
      durum: Durum.Red.toString(),
      createdDateStart: '',
      createdDateEnd: '',
      pageNumber: 1,
      recordsPerPage: RecordsPerPage
    }

  }

  ngOnInit(): void {
    this.getAll();
  }

  filterChanged() {
    this.getAll();
  }

  getAll(pageNumber: number = 1) {

    this.kasaHaraketleriFilter.pageNumber = pageNumber;

    this.kasaHaraketleriService.getAllWithPaging(this.kasaHaraketleriFilter).subscribe((response) => {
      if (response.success) {
        this.kasaHaraketleriList = response.result;
        this.activePage = pageNumber;
        this.totalPages = response.totalPages;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  downloadExcel() {
    this.kasaHaraketleriService.exportToExcel(this.kasaHaraketleriFilter).subscribe((response) => {
      if (response.success) {

        var binaryData = atob(response.result.base64String);

        const blob = new Blob([new Uint8Array(binaryData.length).map((_, index) => binaryData.charCodeAt(index))], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = response.result.fileName;
        a.click();

        window.URL.revokeObjectURL(url);

      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

}
