export interface ChangePasswordDto {
    oldPassword: string;
    newPassword: string;
    newPasswordConfirm: string;
}