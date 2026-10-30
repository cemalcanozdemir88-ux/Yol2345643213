import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { provideEnvironmentNgxCurrency, NgxCurrencyInputMode, NgxCurrencyDirective} from 'ngx-currency';
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
    ChangepasswordComponent
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
    provideEnvironmentNgxCurrency({
      align: 'right',
      allowNegative: true,
      allowZero: true,
      decimal: '.',
      precision: 2,
      prefix: '',
      suffix: '',
      thousands: ',',
      nullable: true,
      min: null,
      max: null,
      inputMode: NgxCurrencyInputMode.Natural,
    }),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
