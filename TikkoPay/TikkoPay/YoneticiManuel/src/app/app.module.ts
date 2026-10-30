import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { provideEnvironmentNgxCurrency, NgxCurrencyInputMode, NgxCurrencyDirective } from 'ngx-currency';
import { NumberRangePipe } from './_pipes/number-range.pipe';
import { KullaniciYetkileriFilterPipe } from './_pipes/kullanici-yetkileri-filter.pipe';
import { HasAuthorizationDirective } from './_directives/hasauthorization.directive';
import { appRoutes } from './routes';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { HeaderComponent } from './header/header.component';
import { BankaComponent } from './banka/banka.component';
import { BekleyenCekimlerComponent } from './bekleyen-cekimler/bekleyen-cekimler.component';
import { BekleyenYatirimlarComponent } from './bekleyen-yatirimlar/bekleyen-yatirimlar.component';
import { HavaleBankalariComponent } from './havale-bankalari/havale-bankalari.component';
import { HomeComponent } from './home/home.component';
import { IptalCekimlerComponent } from './iptal-cekimler/iptal-cekimler.component';
import { IptalYatirimlarComponent } from './iptal-yatirimlar/iptal-yatirimlar.component';

import { KasaHaraketleriComponent } from './kasa-haraketleri/kasa-haraketleri.component';
import { KullanicilarComponent } from './kullanicilar/kullanicilar.component';
import { LoginComponent } from './login/login.component';
import { OnayliCekimlerComponent } from './onayli-cekimler/onayli-cekimler.component';
import { OnayliYatirimlarComponent } from './onayli-yatirimlar/onayli-yatirimlar.component';
import { TedarikciComponent } from './tedarikci/tedarikci.component';
import { WebSiteleriComponent } from './web-siteleri/web-siteleri.component';
import { KasaHaraketleriTedarikciBakiyeComponent } from './kasa-haraketleri-tedarikci-bakiye/kasa-haraketleri-tedarikci-bakiye.component';
import { WebSiteBakiyeComponent } from './web-site-bakiye/web-site-bakiye.component';
import { NoAuthComponent } from './no-auth/no-auth.component';
import { AffiliateComponent } from './affiliate/affiliate.component';
import { AffiliateBakiyeComponent } from './affiliate-bakiye/affiliate-bakiye.component';
import { TwoFactorAuthenticatorComponent } from './two-factor-authenticator/two-factor-authenticator.component';

import { TumYatirimlarComponent } from './tum-yatirimlar/tum-yatirimlar.component';
import { IptalTalepleriComponent } from './iptal-talepleri/iptal-talepleri.component';
import { PaginationComponent } from './pagination/pagination.component';
import { KasaHaraketleriSiteComponent } from './kasa-haraketleri-site/kasa-haraketleri-site.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { KullaniciBlacklistComponent } from './kullanici-blacklist/kullanici-blacklist.component';
import { InvalidIpAddressComponent } from './invalid-ip-address/invalid-ip-address.component';
import { KullaniciIpAdresleriComponent } from './kullanici-ip-adresleri/kullanici-ip-adresleri.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { TumCekimlerComponent } from './tum-cekimler/tum-cekimler.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HeaderComponent,
    HomeComponent,
    TedarikciComponent,
    LoginComponent,
    BankaComponent,
    HavaleBankalariComponent,
    KullanicilarComponent,
    BekleyenYatirimlarComponent,
    BekleyenCekimlerComponent,
    OnayliCekimlerComponent,
    OnayliYatirimlarComponent,
    IptalTalepleriComponent,
    KasaHaraketleriComponent,
    IptalCekimlerComponent,
    IptalYatirimlarComponent,
    WebSiteleriComponent,
    KasaHaraketleriTedarikciBakiyeComponent,
    WebSiteBakiyeComponent,
    NumberRangePipe,
    KullaniciYetkileriFilterPipe,
    PaginationComponent,
    HasAuthorizationDirective,
    NoAuthComponent,
    AffiliateComponent,
    AffiliateBakiyeComponent,
    TwoFactorAuthenticatorComponent,
    TumYatirimlarComponent,
    KasaHaraketleriSiteComponent,
    ChangePasswordComponent,
    KullaniciBlacklistComponent,
    InvalidIpAddressComponent,
    KullaniciIpAdresleriComponent,
    TumCekimlerComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgxCurrencyDirective,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    provideEnvironmentNgxCurrency({
      align: "right",
      allowNegative: true,
      allowZero: true,
      decimal: ".",
      precision: 2,
      prefix: "TRY ",
      suffix: "",
      thousands: ",",
      nullable: true,
      min: null,
      max: null,
      inputMode: NgxCurrencyInputMode.Financial,
    }),
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
