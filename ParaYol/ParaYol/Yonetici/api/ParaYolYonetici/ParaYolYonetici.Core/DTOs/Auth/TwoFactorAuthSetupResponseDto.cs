namespace ParaYolYonetici.Core.DTOs.Auth
{
    public class TwoFactorAuthSetupResponseDto
    {
        public string BarcodeImageUrl { get; set; }
        public string SetupCode { get; set; }
    }
}
