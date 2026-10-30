import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { KullaniciTip } from '../_enums/kullaniciTip.enum';
import { Authorization } from '../_constants/authorizations';

declare var $: any;

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styles: [
  ]
})
export class NavbarComponent implements OnInit {

  Authorization = Authorization;

  constructor(private authService: AuthService) {

  }

  ngOnInit(): void {
  }

  ngAfterViewInit() {

    var pathname = window.location.pathname;
    var targetElement = $('#side-menu a[href="' + pathname + '"]').parent("li").parent("ul").parent("li");
    targetElement.addClass("active");

    $('#side-menu').metisMenu();
  }


  getName(): string {
    return this.authService.getName();
  }

  isTedarikci() {
    return this.authService.getRole() == KullaniciTip.TedarikciKullanicisi.toString();
  }

  userHasAuthorization(authorizationId: string) {
    return this.authService.userHasAuthorization(authorizationId);
  }

  userHasAnyOfAuthorization(authorizationIds: string[]): boolean {
    for (let i = 0; i < authorizationIds.length; i++) {
      if (this.userHasAuthorization(authorizationIds[i])) {
        return true
      }
    }
    return false;
  }

}
