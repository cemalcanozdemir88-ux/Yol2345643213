import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { LoginComponent } from './login/login.component';
import { NavbarComponent } from './navbar/navbar.component';
import { HeaderComponent } from './header/header.component';
import { HomeComponent } from './home/home.component';
import { PaginationComponent } from './pagination/pagination.component';
import { NumberRangePipe } from './_pipes/number-range.pipe';
import { HavaleBankalariComponent } from './havale-bankalari/havale-bankalari.component';
import { TedarikciKullaniciYonetimiComponent } from './tedarikci-kullanici-yonetimi/tedarikci-kullanici-yonetimi.component';
import { BekleyenYatirimlarComponent } from './yatirim/bekleyen-yatirimlar/bekleyen-yatirimlar.component';
import { OnaylananYatirimlarComponent } from './yatirim/onaylanan-yatirimlar/onaylanan-yatirimlar.component';
import { IptalYatirimlarComponent } from './yatirim/iptal-yatirimlar/iptal-yatirimlar.component';
import { BekleyenCekimlerComponent } from './cekim/bekleyen-cekimler/bekleyen-cekimler.component';
import { OnaylananCekimlerComponent } from './cekim/onaylanan-cekimler/onaylanan-cekimler.component';
import { IptalCekimlerComponent } from './cekim/iptal-cekimler/iptal-cekimler.component';
import { KasaHaraketleriBekleyenComponent } from './kasa-haraketleri/kasa-haraketleri-bekleyen/kasa-haraketleri-bekleyen.component';
import { KasaHaraketleriOnaylananComponent } from './kasa-haraketleri/kasa-haraketleri-onaylanan/kasa-haraketleri-onaylanan.component';
import { KasaHaraketleriRedComponent } from './kasa-haraketleri/kasa-haraketleri-red/kasa-haraketleri-red.component';
import { TwoFactorAuthenticatorComponent } from './two-factor-authenticator/two-factor-authenticator.component';
import { ChangepasswordComponent } from './profile/changepassword/changepassword.component';
import { ErrorInterceptor } from './_interceptors/error.interceptor';
import { BekleyenYatirimlarManualComponent } from './manual-yatirim/bekleyen-yatirimlar-manual/bekleyen-yatirimlar-manual.component';
import { OnaylananYatirimlarManualComponent } from './manual-yatirim/onaylanan-yatirimlar-manual/onaylanan-yatirimlar-manual.component';
import { IptalYatirimlarManualComponent } from './manual-yatirim/iptal-yatirimlar-manual/iptal-yatirimlar-manual.component';
import { TedarikciMutabakatComponent } from './tedarikci-mutabakat/tedarikci-mutabakat.component';
import {
  NGX_CURRENCY_CONFIG,
  NgxCurrencyDirective,
  NgxCurrencyInputMode
} from 'ngx-currency';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    NavbarComponent,
    HeaderComponent,
    HomeComponent,
    PaginationComponent,
    NumberRangePipe,
    HavaleBankalariComponent,
    TedarikciKullaniciYonetimiComponent,
    KasaHaraketleriBekleyenComponent,
    KasaHaraketleriOnaylananComponent,
    KasaHaraketleriRedComponent,
    BekleyenYatirimlarComponent,
    OnaylananYatirimlarComponent,
    IptalYatirimlarComponent,
    BekleyenCekimlerComponent,
    OnaylananCekimlerComponent,
    IptalCekimlerComponent,
    TwoFactorAuthenticatorComponent,
    ChangepasswordComponent,
    BekleyenYatirimlarManualComponent,
    OnaylananYatirimlarManualComponent,
    IptalYatirimlarManualComponent,
    TedarikciMutabakatComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgxCurrencyDirective,
    RouterModule.forRoot(appRoutes),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    {
      provide: NGX_CURRENCY_CONFIG,
      useValue: {
        align: 'right',
        allowNegative: true,
        allowZero: true,
        decimal: '.',
        precision: 2,
        prefix: 'TRY ',
        suffix: '',
        thousands: ',',
        nullable: true,
        min: null,
        max: null,
        inputMode: NgxCurrencyInputMode.Financial
      }
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
