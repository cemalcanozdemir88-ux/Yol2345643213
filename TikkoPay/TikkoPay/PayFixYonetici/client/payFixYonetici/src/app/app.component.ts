import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  constructor(private authService: AuthService) {}

  private tokenKey = 'refreshToken';
  title: string = "payFixYonetici";

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.authService.setDecodedToken();
    }
  }

  private generateToken(): string {
    return Math.random().toString(36).substring(2);
  }

  isLogged() {
    if (this.authService.loggedIn()) {
      return true;
    } else {
      return false;
    }
  }
}
