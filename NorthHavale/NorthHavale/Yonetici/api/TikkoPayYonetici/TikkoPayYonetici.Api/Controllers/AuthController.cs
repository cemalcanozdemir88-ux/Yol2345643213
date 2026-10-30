using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TikkoPayYonetici.Api.Infrastructure;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.Auth;
using TikkoPayYonetici.Core.DTOs.Kullanicilar;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenSettings _jwtTokenSettings;
        private readonly IKullanicilarService _kullanicilarService;
        private readonly IKullaniciYetkileriService _kullaniciYetkileriService;
        private readonly OtpSettings _otpSettings;
        private readonly IKullaniciOtpValuesRepository _kullaniciOtpValuesRepository;
        private readonly IKullaniciLoginRepository _kullaniciLoginRepository;
        private readonly IKullaniciIpAdresleriService _kullaniciIpAdresleriService;

        public AuthController(IOptions<JwtTokenSettings> options,
                              IKullanicilarService kullanicilarService,
                              IKullaniciYetkileriService kullaniciYetkileriService,
                              IOptions<OtpSettings> otpSettingsOptions,
                              IKullaniciOtpValuesRepository kullaniciOtpValuesRepository,
                              IKullaniciLoginRepository kullaniciLoginRepository,
                              IKullaniciIpAdresleriService kullaniciIpAdresleriService)
        {
            _jwtTokenSettings = options.Value;
            _kullanicilarService = kullanicilarService;
            _kullaniciYetkileriService = kullaniciYetkileriService;
            _otpSettings = otpSettingsOptions.Value;
            _kullaniciOtpValuesRepository = kullaniciOtpValuesRepository;
            _kullaniciLoginRepository = kullaniciLoginRepository;
            _kullaniciIpAdresleriService = kullaniciIpAdresleriService;
        }

        [HttpPost("login-otp")]
        public async Task<IActionResult> LoginOtp([FromBody] LoginRequest loginRequest)
        {
            var result = await _kullanicilarService.GetByEmailPassword(loginRequest);
            if (!result.Success)
            {
                await AddKullaniciLoginLog(loginRequest.Eposta, "Kullanıcı bulunamadı!", "LoginOtp method satır: 47", false);
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
                await AddKullaniciLoginLog($"id: {id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 71", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            TimeSpan difference = DateTime.Now.ToUniversalTime() - kullaniciOtpValueItem.KayitTarihi.ToUniversalTime();
            if (difference.TotalSeconds > 30)
            {
                await AddKullaniciLoginLog($"id: {id}", "Talep zaman aşımına uğradı!", "GetOtpBarcode method satır: 78", false);
                return NotFound("Talep zaman aşımına uğradı!");
            }

            var userItem = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (userItem == null)
            {
                await AddKullaniciLoginLog($"id: {id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 85", false);
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
                await AddKullaniciLoginLog($"validateOtpCodeRequest.Id: {validateOtpCodeRequest.Id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 115", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            var kullanici = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (kullanici == null)
            {
                await AddKullaniciLoginLog($"kullaniciOtpValueItem.KullaniciId: {kullaniciOtpValueItem.KullaniciId}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 122", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(kullanici);
            int validationWindow = 1;

            var TwoFacAuth = new TwoFactorAuthenticator();
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(userUniqueKey, validateOtpCodeRequest.Code, validationWindow);
            if (!isValid)
            {
                await AddKullaniciLoginLog(kullanici.Result.Eposta, "Kod hatalı!", "GetOtpBarcode method satır: 133", false);
                return NotFound("Kod hatalı!");
            }

            await _kullanicilarService.SetOtpEnabled(kullanici.Result.Id);

            var kullaniciYetkileri = await _kullaniciYetkileriService.GetByKullaniciId(kullanici.Result.Id);

            var token = JwtTokenGenerator.GenerateToken(kullanici.Result, kullaniciYetkileri.Result, _jwtTokenSettings);

            if (kullanici.Result.IpLoggingEnabled)
            {
                //await AddKullaniciLoginIp(kullanici.Result.Id);
                await AddKullaniciLoginLog(kullanici.Result.Eposta, "", "", true);
            }

            return Ok(token);
        }

        private static string GetIssuer(ResponseViewModel<KullanicilarListDto> userItem)
        {
            //if (userItem.Result.KullaniciTipi == Core.Enums.KullaniciTip.WebSiteKullanicisi)
            //{
            //    return "bayi.thyhometurnnn.com";
            //}
            //else if (userItem.Result.KullaniciTipi == Core.Enums.KullaniciTip.AffiliateKullanicisi)
            //{
            //    return "affiliate.thyhometursss.com";
            //}

            //return "yonetici.tikkopay.com";

            return "bayi.thyhometurnnn.com";
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        private string GetUserUniqueKey(ResponseViewModel<KullanicilarListDto> userItem)
        {
            string googleAuthKey = _otpSettings.AuthSecretKey;
            return $"{GetIssuer(userItem)}{userItem.Result.Eposta}{googleAuthKey}";
        }

        private async Task AddKullaniciLoginLog(string email, string errorMessage, string detailMessage, bool isSuccess)
        {
            await _kullaniciLoginRepository.AddAsync(new KullaniciLogin
            {
                Id = Guid.NewGuid(),
                Eposta = email,
                ErrorMessage = errorMessage,
                DetailMessage = detailMessage,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsSuccess = isSuccess
            });
        }

        private async Task AddKullaniciLoginIp(Guid kullaniciId)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _kullaniciIpAdresleriService.CreateIfAvaliable(new Core.DTOs.KullaniciIpAdresleri.KullaniciIpAdresleriCreateDto
            {
                KullaniciId = kullaniciId,
                IpAdresi = ipAddress ?? string.Empty,
            });
        }
    }
}
