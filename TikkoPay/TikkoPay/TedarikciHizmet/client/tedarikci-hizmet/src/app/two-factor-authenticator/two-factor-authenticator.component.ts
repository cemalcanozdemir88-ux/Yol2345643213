import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { SignalrService } from '../_services/signalr.service';
import { ValidateOtpCodeRequest } from '../_models/auth/validateOtpCodeRequest';

@Component({
  selector: 'app-two-factor-authenticator',
  templateUrl: './two-factor-authenticator.component.html',
  styleUrls: ['./two-factor-authenticator.component.css']
})
export class TwoFactorAuthenticatorComponent {

  public hasError: boolean = false;
  public errorText: string = "";

  public isLoading: boolean = false;
  validateOtpRequest!: ValidateOtpCodeRequest;

  barcodeImageUrl: string = "";
  setupCode: string = "";
  enteredCode: string = "";

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private route: ActivatedRoute,
    private signalr: SignalrService
  ) {
    this.validateOtpRequest = {
      code: '',
      id: '',
    };
  }
  
  ngOnInit(): void {
    this.getOtpBarcode();
  }

  getOtpBarcode(){

    const id = this.route.snapshot.queryParamMap.get('id');
    if (!id) {
      this.alertify.error('ID parametresi eksik.');
      return;
    }

    this.authService.getOtpBarcode(id).subscribe({
      next: response => {
        this.barcodeImageUrl = response.result.barcodeImageUrl;
        this.setupCode = response.result.setupCode;
      },
      error: error => {
        this.hasError = true;
        this.errorText = error.error;
        
      },
      complete: () => {
      }
    });
  }

  onSubmit(){

    const id = this.route.snapshot.queryParamMap.get('id');
    if (!id) {
      this.alertify.error('ID parametresi eksik.');
      return;
    }

    if (!this.enteredCode) {
      this.alertify.error('Code boş geçilemez');
      return;
    }

    this.enteredCode = this.enteredCode.trim();

    this.validateOtpRequest.code = this.enteredCode;
    this.validateOtpRequest.id = id;

    this.isLoading = true;
    this.authService.sendOtpCode(this.validateOtpRequest).subscribe({
      next: response => {
        this.isLoading = false;
        // Start SignalR now that token is stored by AuthService
        this.signalr.startConnection();

        this.router.navigate(['/tedarikci-bekleyen-yatirimlar']);
      },
      error: error => {
        this.isLoading = false;
        
      },
      complete: () => {

      }
    });
  }
}
