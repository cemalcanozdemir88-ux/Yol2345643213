import { Component } from '@angular/core';
import { LoginRequest } from '../_models/kullanicilar/loginRequest';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styles: [
  ]
})
export class LoginComponent {

  public isLoading: boolean = false;
  user!: LoginRequest;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) {

    this.user = {
      eposta: '',
      sifre: ''
    };

    this.authService.logout();
  }

  onSubmit() {

    if (!this.user.eposta) {
      this.alertify.error("E-posta boş geçilemez");
      return;
    }
    if (!this.user.sifre) {
      this.alertify.error("Şifre boş geçilemez");
      return;
    }

    this.user.eposta = this.user.eposta.trim();
    this.user.sifre = this.user.sifre.trim();

    this.isLoading = true;

    // this.authService.login(this.user).subscribe({
    //   next: response => {
    //     this.isLoading = false;
        
    //     if (this.isWebSiteKullanicisi()) {
    //       this.router.navigate(['/website-bakiye']);
    //     }
    //     else if(this.isAffiliateKullanicisi()){
    //       this.router.navigate(['/affiliate-bakiye']);
    //     }
    //     else {
    //       this.router.navigate(['/']);
    //     }
    //   },
    //   error: error => {
    //     this.isLoading = false;
    //     this.alertify.error(error.error);
    //   },
    //   complete: () => {

    //   }
    // });

    this.authService.loginOtp(this.user).subscribe({
      next: (response) => {
       
        this.isLoading = false;
        this.router.navigate(['/2fa'], {queryParams: { id: response.result.id } });
        
      },
      error: (error) => {
        this.isLoading = false;
        this.alertify.error(error.error);
        this.user = {
          eposta: '',
          sifre: ''
        };
      },
      complete: () => {},
    });
  }

  isWebSiteKullanicisi() {
    return this.authService.getRole() == KullaniciTip.WebSiteKullanicisi.toString();
  }

  isAffiliateKullanicisi() {
    return this.authService.getRole() == KullaniciTip.AffiliateKullanicisi.toString();
  }
}
