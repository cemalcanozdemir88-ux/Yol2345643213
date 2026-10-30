import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BankaComponent } from './banka/banka.component';
import { BekleyenCekimlerComponent } from './bekleyen-cekimler/bekleyen-cekimler.component';
import { BekleyenYatirimlarComponent } from './bekleyen-yatirimlar/bekleyen-yatirimlar.component';
import { HavaleBankalariComponent } from './havale-bankalari/havale-bankalari.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { IptalCekimlerComponent } from './iptal-cekimler/iptal-cekimler.component';
import { IptalTalepleriComponent } from './iptal-talepleri/iptal-talepleri.component';
import { IptalYatirimlarComponent } from './iptal-yatirimlar/iptal-yatirimlar.component';
import { KasaHaraketleriSiteComponent } from './kasa-haraketleri-site/kasa-haraketleri-sitecomponent';
import { KasaHaraketleriComponent } from './kasa-haraketleri/kasa-haraketleri.component';
import { KullanicilarComponent } from './kullanicilar/kullanicilar.component';
import { LoginComponent } from './login/login.component';
import { NavbarComponent } from './navbar/navbar.component';
import { OnayliCekimlerComponent } from './onayli-cekimler/onayli-cekimler.component';
import { OnayliYatirimlarComponent } from './onayli-yatirimlar/onayli-yatirimlar.component';
import { appRoutes } from './routes';
import { TedarikciComponent } from './tedarikci/tedarikci.component';
import { WebSiteleriComponent } from './web-siteleri/web-siteleri.component';
import { KasaHaraketleriTedarikciBakiyeComponent } from './kasa-haraketleri-tedarikci-bakiye/kasa-haraketleri-tedarikci-bakiye.component';
import { provideEnvironmentNgxCurrency, NgxCurrencyInputMode, NgxCurrencyDirective } from 'ngx-currency';
import { WebSiteBakiyeComponent } from './web-site-bakiye/web-site-bakiye.component';
import { NumberRangePipe } from './_pipes/number-range.pipe';
import { PaginationComponent } from './pagination/pagination.component';
import { KullaniciYetkileriFilterPipe } from './_pipes/kullanici-yetkileri-filter.pipe';
import { HasAuthorizationDirective } from './_directives/hasauthorization.directive';
import { NoAuthComponent } from './no-auth/no-auth.component';
import { AffiliateComponent } from './affiliate/affiliate.component';
import { AffiliateBakiyeComponent } from './affiliate-bakiye/affiliate-bakiye.component';
import { TwoFactorAuthenticatorComponent } from './two-factor-authenticator/two-factor-authenticator.component';
import { ChangepasswordComponent } from './profile/changepassword/changepassword.component';


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
    KasaHaraketleriSiteComponent,
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
    ChangepasswordComponent,
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
