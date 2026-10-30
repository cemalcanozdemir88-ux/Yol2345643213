using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayCoYonetici.Api.Infrastructure;
using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.Auth;
using PayCoYonetici.Core.DTOs.Kullanicilar;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Core.Models;
using PayCoYonetici.Services.Interfaces;
using System.Text;

namespace PayCoYonetici.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IKullanicilarService _kullanicilarService;
        private readonly IKullaniciOtpValuesRepository _kullaniciOtpValuesRepository;
        private readonly OtpSettings _otpSettings;
        private readonly IKullaniciYetkileriService _kullaniciYetkileriService;
        private readonly JwtTokenSettings _jwtTokenSettings;

        public AuthController(IKullanicilarService kullanicilarService,
                              IKullaniciOtpValuesRepository kullaniciOtpValuesRepository,
                              IOptions<OtpSettings> otpSettingsOptions,
                              IKullaniciYetkileriService kullaniciYetkileriService,
                              IOptions<JwtTokenSettings> options)
        {
            _kullanicilarService = kullanicilarService;
            _kullaniciOtpValuesRepository = kullaniciOtpValuesRepository;
            _otpSettings = otpSettingsOptions.Value;
            _kullaniciYetkileriService = kullaniciYetkileriService;
            _jwtTokenSettings = options.Value;
        }

        [HttpPost("login-otp")]
        public async Task<IActionResult> LoginOtp([FromBody] LoginRequest loginRequest)
        {
            var result = await _kullanicilarService.GetByEmailPassword(loginRequest);
            if (!result.Success)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            var kullaniciOtpId = await _kullaniciOtpValuesRepository.Add(result.Result.Id);

            var response = new ResponseViewModel<LoginOtpResponseDto>
            {
                Success = true,
                Result = new LoginOtpResponseDto
                {
                    Id = kullaniciOtpId,
                }
            };

            return Ok(response);
        }

        [HttpGet("get-otp-barcode/{id}")]
        public async Task<IActionResult> GetOtpBarcode(string id)
        {
            var kullaniciOtpValueItem = await _kullaniciOtpValuesRepository.GetById(Guid.Parse(id));
            if (kullaniciOtpValueItem == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            TimeSpan difference = DateTime.Now.ToUniversalTime() - kullaniciOtpValueItem.KayitTarihi.ToUniversalTime();
            if (difference.TotalSeconds > 30)
            {
                return NotFound("Talep zaman aşımına uğradı!");
            }

            var userItem = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (userItem == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(userItem);
            var twoFacAuth = new TwoFactorAuthenticator();

            string issuer = GetIssuer(userItem);

            var setupInfo = twoFacAuth.GenerateSetupCode(issuer, userItem.Result.Eposta, ConvertSecretToBytes(userUniqueKey, false), 300);

            var response = new ResponseViewModel<TwoFactorAuthSetupResponseDto>
            {
                Success = true,
                Result = new TwoFactorAuthSetupResponseDto
                {
                    BarcodeImageUrl = userItem.Result.IsOtpEnabled ? string.Empty : setupInfo.QrCodeSetupImageUrl,
                    SetupCode = userItem.Result.IsOtpEnabled ? string.Empty : setupInfo.ManualEntryKey
                }
            };

            return Ok(response);
        }

        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateOtpCode([FromBody] ValidateOtpCodeRequest validateOtpCodeRequest)
        {
            var kullaniciOtpValueItem = await _kullaniciOtpValuesRepository.GetById(Guid.Parse(validateOtpCodeRequest.Id));
            if (kullaniciOtpValueItem == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            var kullanici = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (kullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(kullanici);
            int validationWindow = 1;

            var TwoFacAuth = new TwoFactorAuthenticator();
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(userUniqueKey, validateOtpCodeRequest.Code, validationWindow);
            if (!isValid)
            {
                return NotFound("Kod hatalı!");
            }

            await _kullanicilarService.SetOtpEnabled(kullanici.Result.Id);

            var kullaniciYetkileri = await _kullaniciYetkileriService.GetByKullaniciId(kullanici.Result.Id);

            var token = JwtTokenGenerator.GenerateToken(kullanici.Result, kullaniciYetkileri.Result, _jwtTokenSettings);
            return Ok(token);
        }

        private static string GetIssuer(ResponseViewModel<KullanicilarListDto> userItem)
        {
            if (userItem.Result.KullaniciTipi == Core.Enums.KullaniciTip.WebSiteKullanicisi)
            {
                return "bayi-payco.tedarikcihizmet.com";
            }
            else if (userItem.Result.KullaniciTipi == Core.Enums.KullaniciTip.AffiliateKullanicisi)
            {
                return "affiliate-payco.tedarikcihizmet.com";
            }

            return "payco-yonetici.tikkopay.com";
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
          secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        private string GetUserUniqueKey(ResponseViewModel<KullanicilarListDto> userItem)
        {
            string googleAuthKey = _otpSettings.AuthSecretKey;
            return $"{GetIssuer(userItem)}{userItem.Result.Eposta}{googleAuthKey}";
        }
    }
}
