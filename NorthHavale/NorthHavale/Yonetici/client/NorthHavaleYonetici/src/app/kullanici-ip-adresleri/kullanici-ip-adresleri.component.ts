import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Authorization } from '../_constants/authorizations';
import { KullaniciIpAdresleriListDto } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriList.dto';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { KullaniciIpAdresleriFilter } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriFilter';
import { AlertifyService } from '../_services/alertify.service';
import { TedarikciService } from '../_services/tedarikci.service';
import { KullaniciIpAdresleriService } from '../_services/kullaniciIpAdresleri.service';

@Component({
  selector: 'app-kullanici-ip-adresleri',
  templateUrl: './kullanici-ip-adresleri.component.html',
  styleUrls: ['./kullanici-ip-adresleri.component.css']
})
export class KullaniciIpAdresleriComponent implements OnInit {

  Authorization = Authorization;

  @ViewChild('kullaniciIpAdresleriSilModalKapatButton') kullaniciIpAdresleriSilModalKapatButton!: ElementRef;

  tedarikcilerDronDownModel: TedarikcilerListDto[] = [];

  kullaniciIpAdresleri: KullaniciIpAdresleriListDto[] = [];
  kullaniciIpAdresleriFilter!: KullaniciIpAdresleriFilter;

  public isDeleting: boolean = false;
  selectedItemId: string = "";

  constructor(private kullaniciIpAdresleriService: KullaniciIpAdresleriService,
    private tedarikcilerService: TedarikciService,
    private alertify: AlertifyService) {

    this.kullaniciIpAdresleriFilter = {
      tedarikciId: '',
      searchText: '',
    }

  }

  ngOnInit(): void {
    this.getTedarikciler();
    this.getAll();
  }

  filterChanged() {
    this.getAll();
  }

  getTedarikciler() {
    this.tedarikcilerService.getAll().subscribe((reponse) => {
      this.tedarikcilerDronDownModel = reponse.result;
    });
  }

  getAll() {

    this.kullaniciIpAdresleriService.getAll(this.kullaniciIpAdresleriFilter).subscribe((response) => {
      if (response.success) {
        this.kullaniciIpAdresleri = response.result;
      }
      else {
        this.alertify.error(response.message);
      }

    })
  }

  closeSilModal() {
    if (this.kullaniciIpAdresleriSilModalKapatButton && this.kullaniciIpAdresleriSilModalKapatButton.nativeElement) {
      this.kullaniciIpAdresleriSilModalKapatButton.nativeElement.click();
    }
  }


  cancelDelete() {
    this.selectedItemId = "";
  }

  startDelete(itemId: string) {
    this.selectedItemId = itemId;
  }

  deleteRecord() {
    if (this.selectedItemId !== "") {
      this.isDeleting = true;
      this.kullaniciIpAdresleriService.delete(this.selectedItemId).subscribe({
        next: (response) => {
          this.getAll();

          if (response.success) {
            this.alertify.success(response.message);
          }
          else {
            this.alertify.error(response.message);
          }
        },
        error: error => {
          console.log(error);
        },
        complete: () => {
          this.selectedItemId = "";
          this.isDeleting = false;
          this.closeSilModal();
        }
      });
    }
  }


}
