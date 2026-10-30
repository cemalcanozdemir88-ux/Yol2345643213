import { Routes } from "@angular/router";
import { authGuard } from "./_guard/auth.guard";
import { roleGuard } from "./_guard/role.guard";
import { BankaComponent } from "./banka/banka.component";
import { BekleyenCekimlerComponent } from "./bekleyen-cekimler/bekleyen-cekimler.component";
import { BekleyenYatirimlarComponent } from "./bekleyen-yatirimlar/bekleyen-yatirimlar.component";
import { HavaleBankalariComponent } from "./havale-bankalari/havale-bankalari.component";
import { HomeComponent } from "./home/home.component";
import { IptalCekimlerComponent } from "./iptal-cekimler/iptal-cekimler.component";
import { IptalYatirimlarComponent } from "./iptal-yatirimlar/iptal-yatirimlar.component";
import { KasaHaraketleriSiteComponent } from "./kasa-haraketleri-site/kasa-haraketleri-site.component";
import { KasaHaraketleriComponent } from "./kasa-haraketleri/kasa-haraketleri.component";
import { KullanicilarComponent } from "./kullanicilar/kullanicilar.component";
import { LoginComponent } from "./login/login.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { OnayliCekimlerComponent } from "./onayli-cekimler/onayli-cekimler.component";
import { OnayliYatirimlarComponent } from "./onayli-yatirimlar/onayli-yatirimlar.component";
import { TedarikciComponent } from "./tedarikci/tedarikci.component";
import { WebSiteleriComponent } from "./web-siteleri/web-siteleri.component";
import { KasaHaraketleriTedarikciBakiyeComponent } from "./kasa-haraketleri-tedarikci-bakiye/kasa-haraketleri-tedarikci-bakiye.component";
import { KullaniciTip } from "./_enums/kullaniciTip.enum";
import { WebSiteBakiyeComponent } from "./web-site-bakiye/web-site-bakiye.component";
import { Authorization } from "./_constants/authorizations";
import { NoAuthComponent } from "./no-auth/no-auth.component";
import { AffiliateComponent } from "./affiliate/affiliate.component";
import { AffiliateBakiyeComponent } from "./affiliate-bakiye/affiliate-bakiye.component";
import { TwoFactorAuthenticatorComponent } from "./two-factor-authenticator/two-factor-authenticator.component";

import { TumYatirimlarComponent } from "./tum-yatirimlar/tum-yatirimlar.component";
import { ChangePasswordComponent } from "./change-password/change-password.component";
import { KullaniciBlacklistComponent } from "./kullanici-blacklist/kullanici-blacklist.component";
import { InvalidIpAddressComponent } from "./invalid-ip-address/invalid-ip-address.component";
import { KullaniciIpAdresleriComponent } from "./kullanici-ip-adresleri/kullanici-ip-adresleri.component";
import { TumCekimlerComponent } from "./tum-cekimler/tum-cekimler.component";


export const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [authGuard] },
    { path: 'navbar', component: NavbarComponent, canActivate: [authGuard] },
    { path: 'home', component: HomeComponent, canActivate: [authGuard] },

    { path: 'bekleyenyatirimlar', component: BekleyenYatirimlarComponent, canActivate: [authGuard], data: { requiredRoles: [KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.BekleyenYatirimlarGorebilir } },
    { path: 'onayliyatirimlar', component: OnayliYatirimlarComponent, canActivate: [authGuard], data: { requiredRoles: [KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.OnayliYatirimlarGorebilir } },
    { path: 'iptalyatirimtalepleri', component: IptalYatirimlarComponent, canActivate: [authGuard], data: { requiredRoles: [KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.IptalEdilenYatirimlarGorebilir } },

    { path: 'havalebankalari', component: HavaleBankalariComponent, canActivate: [authGuard], data: { requiredRoles: [KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.HavaleBankalariGorebilir } },

    { path: 'profile/changepassword', component: ChangePasswordComponent, canActivate: [authGuard] },

    { path: 'login', component: LoginComponent },
    { path: 'invalid-ip', component: InvalidIpAddressComponent },
    { path: '2fa', component: TwoFactorAuthenticatorComponent },
    { path: 'no-auth', component: NoAuthComponent }
]