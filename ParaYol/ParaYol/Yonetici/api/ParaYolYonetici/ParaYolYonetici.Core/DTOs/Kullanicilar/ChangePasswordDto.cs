namespace ParaYolYonetici.Core.DTOs.Kullanicilar
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewPasswordConfirm { get; set; } = string.Empty;
    }
}
