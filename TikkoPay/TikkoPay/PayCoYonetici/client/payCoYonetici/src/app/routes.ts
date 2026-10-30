import { Routes } from "@angular/router";
import { authGuard } from "./_guard/auth.guard";
import { roleGuard } from "./_guard/role.guard";
import { HomeComponent } from "./home/home.component";
import { IptalCekimlerComponent } from "./iptal-cekimler/iptal-cekimler.component";
import { KasaHaraketleriSiteComponent } from "./kasa-haraketleri-site/kasa-haraketleri-sitecomponent";
import { KullanicilarComponent } from "./kullanicilar/kullanicilar.component";
import { LoginComponent } from "./login/login.component";
import { NavbarComponent } from "./navbar/navbar.component";
import { OnayliCekimlerComponent } from "./onayli-cekimler/onayli-cekimler.component";
import { OnayliYatirimlarComponent } from "./onayli-yatirimlar/onayli-yatirimlar.component";
import { WebSiteleriComponent } from "./web-siteleri/web-siteleri.component";
import { KullaniciTip } from "./_enums/kullaniciTip.enum";
import { WebSiteBakiyeComponent } from "./web-site-bakiye/web-site-bakiye.component";
import { Authorization } from "./_constants/authorizations";
import { NoAuthComponent } from "./no-auth/no-auth.component";
import { AffiliateComponent } from "./affiliate/affiliate.component";
import { AffiliateBakiyeComponent } from "./affiliate-bakiye/affiliate-bakiye.component";
import { TwoFactorAuthenticatorComponent } from "./two-factor-authenticator/two-factor-authenticator.component";
import { ChangepasswordComponent } from "./profile/changepassword/changepassword.component";
import { KasaHaraketleriComponent } from "./kasa-haraketleri/kasa-haraketleri.component";
import { KasaHaraketleriBakiyeComponent } from "./kasa-haraketleri-bakiye/kasa-haraketleri-bakiye.component";

export const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [authGuard] },
    { path: 'navbar', component: NavbarComponent, canActivate: [authGuard] },
    { path: 'home', component: HomeComponent, canActivate: [authGuard] },

    { path: 'kasaharaketleri', component: KasaHaraketleriComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString()], requiredAuthorization: Authorization.PayCoKasaHaraketleriGorebilir } },
    { path: 'payco-bakiye', component: KasaHaraketleriBakiyeComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString()], requiredAuthorization: Authorization.PayCoBakiyeGorebilir } },


    { path: 'onayliyatirimlar', component: OnayliYatirimlarComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.OnayliYatirimlarGorebilir } },

    { path: 'onaylicekimler', component: OnayliCekimlerComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.OnayliCekimlerGorebilir } },
    { path: 'iptalcekimtalepleri', component: IptalCekimlerComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.IptalEdilenCekimlerGorebilir } },

    { path: 'kasaharaketlerisite', component: KasaHaraketleriSiteComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.SiteKasaHaraketleriGorebilir } },
    { path: 'webSiteleri', component: WebSiteleriComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString()], requiredAuthorization: Authorization.SiteTanimlariGorebilir } },
    { path: 'website-bakiye', component: WebSiteBakiyeComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.SiteBakiyeGorebilir } },

    { path: 'affiliates', component: AffiliateComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString()], requiredAuthorization: Authorization.AffiliateTanimlariGorebilir } },
    { path: 'affiliate-bakiye', component: AffiliateBakiyeComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.AffiliateKullanicisi.toString()], requiredAuthorization: Authorization.AffiliateBakiyeGorebilir } },

    { path: 'kullanicilar', component: KullanicilarComponent, canActivate: [authGuard, roleGuard], data: { requiredRoles: [KullaniciTip.Admin.toString(), KullaniciTip.WebSiteKullanicisi.toString()], requiredAuthorization: Authorization.KullaniciTanimlariGorebilir } },

    { path: 'profile/changepassword', component: ChangepasswordComponent, canActivate: [authGuard] },

    { path: 'login', component: LoginComponent },
    { path: '2fa', component: TwoFactorAuthenticatorComponent },
    { path: 'no-auth', component: NoAuthComponent }
]