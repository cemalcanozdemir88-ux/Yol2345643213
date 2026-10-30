import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { IptalCekimlerComponent } from './iptal-cekimler/iptal-cekimler.component';
import { KasaHaraketleriSiteComponent } from './kasa-haraketleri-site/kasa-haraketleri-sitecomponent';
import { KullanicilarComponent } from './kullanicilar/kullanicilar.component';
import { LoginComponent } from './login/login.component';
import { NavbarComponent } from './navbar/navbar.component';
import { OnayliCekimlerComponent } from './onayli-cekimler/onayli-cekimler.component';
import { OnayliYatirimlarComponent } from './onayli-yatirimlar/onayli-yatirimlar.component';
import { appRoutes } from './routes';
import { WebSiteleriComponent } from './web-siteleri/web-siteleri.component';
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
import { KasaHaraketleriComponent } from './kasa-haraketleri/kasa-haraketleri.component';
import { KasaHaraketleriBakiyeComponent } from './kasa-haraketleri-bakiye/kasa-haraketleri-bakiye.component';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HeaderComponent,
    HomeComponent,
    LoginComponent,
    KullanicilarComponent,
    OnayliCekimlerComponent,
    OnayliYatirimlarComponent,
    IptalCekimlerComponent,
    KasaHaraketleriSiteComponent,
    WebSiteleriComponent,
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
    KasaHaraketleriComponent,
    KasaHaraketleriBakiyeComponent,
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
