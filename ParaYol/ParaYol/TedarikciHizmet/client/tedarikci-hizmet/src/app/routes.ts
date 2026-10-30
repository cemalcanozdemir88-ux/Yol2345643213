import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NavbarComponent } from './navbar/navbar.component';
import { authGuard } from './_guard/auth.guard';
import { LoginComponent } from './login/login.component';
import { NoAuthComponent } from './no-auth/no-auth.component';
import { tedarikciRoleGuard } from './_guard/tedarikcirole.guard';
import { KullaniciTip } from './_enums/kullaniciTip.enum';
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
import { BekleyenYatirimlarManualComponent } from './manual-yatirim/bekleyen-yatirimlar-manual/bekleyen-yatirimlar-manual.component';
import { OnaylananYatirimlarManualComponent } from './manual-yatirim/onaylanan-yatirimlar-manual/onaylanan-yatirimlar-manual.component';
import { IptalYatirimlarManualComponent } from './manual-yatirim/iptal-yatirimlar-manual/iptal-yatirimlar-manual.component';
import { TedarikciMutabakatComponent } from './tedarikci-mutabakat/tedarikci-mutabakat.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent, canActivate: [authGuard] },
  { path: 'navbar', component: NavbarComponent, canActivate: [authGuard] },
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: '2fa', component: TwoFactorAuthenticatorComponent },
  { path: 'no-auth', component: NoAuthComponent },
  { path: 'profile/changepassword', component: ChangepasswordComponent, canActivate: [authGuard] },

  {
    path: 'tedarikci-bekleyen-yatirimlar',
    component: BekleyenYatirimlarComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-onayli-yatirimlar',
    component: OnaylananYatirimlarComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-iptal-yatirimlar',
    component: IptalYatirimlarComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-bekleyen-cekimler',
    component: BekleyenCekimlerComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-onayli-cekimler',
    component: OnaylananCekimlerComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-iptal-cekimler',
    component: IptalCekimlerComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-bekleyen-yatirimlar-manual',
    component: BekleyenYatirimlarManualComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-onaylanan-yatirimlar-manual',
    component: OnaylananYatirimlarManualComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-iptal-yatirimlar-manual',
    component: IptalYatirimlarManualComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'havale-bankalari',
    component: HavaleBankalariComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-kullanici-yonetimi',
    component: TedarikciKullaniciYonetimiComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },

  {
    path: 'tedarikci-kasaharaketleri-bekleyen',
    component: KasaHaraketleriBekleyenComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-kasaharaketleri-onaylanan',
    component: KasaHaraketleriOnaylananComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  {
    path: 'tedarikci-kasaharaketleri-red',
    component: KasaHaraketleriRedComponent,
    canActivate: [authGuard, tedarikciRoleGuard],
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] },
  },
  { 
    path: 'tedarikci-mutabakat',
    component: TedarikciMutabakatComponent, 
    canActivate: [authGuard, tedarikciRoleGuard], 
    data: { requiredRoles: [KullaniciTip.TedarikciKullanicisi.toString()] } 
  },
];
