import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ChangePasswordDto } from 'src/app/_models/kullanicilar/changePassword.dto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { KullanicilarService } from 'src/app/_services/kullanicilar.service';
import { KullaniciProfileService } from 'src/app/_services/kullaniciprofil.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: []
})
export class ChangePasswordComponent {

  changePasswordModel!: ChangePasswordDto;

  public isCreating: boolean = false;

  constructor(
    private kullaniciProfileService: KullaniciProfileService,
    private alertify: AlertifyService
  ){
    this.createEmptyModel();
  }

  createEmptyModel() {
    this.changePasswordModel = {
      oldPassword: '',
      newPassword: '',
      newPasswordConfirm: ''
    };
  }

  submitChangePasswordForm(kullaniciForm: NgForm){
    if (!kullaniciForm.valid) {
      this.alertify.error("Zorunlu alanları doldurunuz!");
      return;
    }

    this.isCreating = true;

    this.kullaniciProfileService.changePassword(this.changePasswordModel).subscribe({
      next: response => {
        if (response.success) {
          this.createEmptyModel();
          this.alertify.success("Şifre güncellendi");
        }
        else {
          this.alertify.error(response.message);
        }
        this.isCreating = false;
      },
      error: error => {
        this.isCreating = false;
      },
      complete: () => {
      }
    })
  }

}