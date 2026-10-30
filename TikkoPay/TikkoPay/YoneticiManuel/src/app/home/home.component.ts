import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { YatirimTalepleriService } from '../_services/yatirimTalepleri.service';
import { YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { Authorization } from '../_constants/authorizations';
import { CekimTalepleriService } from '../_services/cekimTalepleri.service';
import { CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
  ]
})
export class HomeComponent implements OnInit {

  Authorization = Authorization;

  constructor(
    private authService: AuthService,
    private router: Router) {

      

  }

  ngOnInit(): void {
    

  }

}

