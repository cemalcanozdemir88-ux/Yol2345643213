import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private authService: AuthService) {
    this.clearBrowserCache();
  }

  private tokenKey = 'refreshToken';

  ngOnInit(): void {
    const token = localStorage.getItem("token");
    if (token) {
      this.authService.setDecodedToken();
    }
  }

  clearBrowserCache() {
    if ('caches' in window) {
      caches.keys().then((names) => {
        names.forEach(name => caches.delete(name));
      });
    }
    console.log("Tarayıcı cache temizlendi!");
  }

  private generateToken(): string {
    return Math.random().toString(36).substring(2);
  }

  isLogged() {
    if (this.authService.loggedIn()) {
      return true;
    } else {
      return false
    }
  }
}
