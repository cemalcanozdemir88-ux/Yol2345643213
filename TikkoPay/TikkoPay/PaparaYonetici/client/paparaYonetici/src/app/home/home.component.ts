import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';



@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
  ]
})
export class HomeComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router) {

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
  }

  ngAfterViewInit() {

  }

  // isTedarikci() {
  //   return this.authService.getRole() == KullaniciTip.TedarikciKullanicisi.toString();
  // }

  // isWebSiteKullanicisi() {
  //   return this.authService.getRole() == KullaniciTip.WebSiteKullanicisi.toString();
  // }

}

