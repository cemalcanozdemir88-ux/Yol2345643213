import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  title: string = "Papara";

  constructor(private authService: AuthService) {

  }

  private tokenKey = 'refreshToken';

  ngOnInit(): void {
    const token = localStorage.getItem("token");
    if (token) {
      this.authService.setDecodedToken();
    }
  }

  // ngAfterViewInit(): void{
  //   setTimeout(() => {
  //      const refreshToken = localStorage.getItem(this.tokenKey);

  //     if (!refreshToken) {
  //     const newToken = this.generateToken();
  //     localStorage.setItem(this.tokenKey, newToken);
  //     console.log('Refreshing the page');
  //     location.reload();
  //   } else {
  //     console.log('Page already refreshed');
  //   }
  //   }, 2000);
  // }

 private generateToken(): string {
    return Math.random().toString(36).substring(2);
  }

  isLogged(){
    if (this.authService.loggedIn()) {
      return true;  
    } else {
      return false
    }
  }
}
