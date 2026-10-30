using System.Text;
using AutoMapper;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TedarikciHizmet.Api.Infrastructure;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.Auth;
using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
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
        private readonly IMapper _mapper;
        private readonly IKullaniciLoginRepository _kullaniciLoginRepository;
        private readonly ITedarikciRepository _tedarikciRepository;

        public AuthController(IOptions<JwtTokenSettings> jwtTokenSettingsOptions,
                              IKullanicilarService kullanicilarService,
                              IKullaniciYetkileriService kullaniciYetkileriService,
                              IOptions<OtpSettings> otpSettingsOptions,
                              IKullaniciOtpValuesRepository kullaniciOtpValuesRepository,
                              IMapper mapper,
                              IKullaniciLoginRepository kullaniciLoginRepository,
                              ITedarikciRepository tedarikciRepository)
        {
            _jwtTokenSettings = jwtTokenSettingsOptions.Value;
            _kullanicilarService = kullanicilarService;
            _kullaniciYetkileriService = kullaniciYetkileriService;
            _otpSettings = otpSettingsOptions.Value;
            _kullaniciOtpValuesRepository = kullaniciOtpValuesRepository;
            _mapper = mapper;
            _kullaniciLoginRepository = kullaniciLoginRepository;
            _tedarikciRepository = tedarikciRepository;
        }

        [HttpPost("login-otp")]
        public async Task<IActionResult> LoginOtp([FromBody] LoginRequest loginRequest)
        {
            var result = await _kullanicilarService.GetByEmailPassword(loginRequest);
            if (!result.Success)
            {
                await AddKullaniciLoginLog(loginRequest.Eposta, "Kullanıcı bulunamadı!", "LoginOtp method satır: 51", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            var tedarikci = await _tedarikciRepository.GetByIdAsync(result.Result.TedarikciId.Value);
            if (tedarikci == null)
            {
                await AddKullaniciLoginLog(loginRequest.Eposta, "Kullanıcı bulunamadı! (Tedarikçi Bulunamadı)", "LoginOtp method satır: 62", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            if (!tedarikci.Durum)
            {
                await AddKullaniciLoginLog(loginRequest.Eposta, "Kullanıcı bulunamadı! (Tedarikçi Durum Pasif)", "LoginOtp method satır: 68", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            if (tedarikci.SilindiMi)
            {
                await AddKullaniciLoginLog(loginRequest.Eposta, "Kullanıcı bulunamadı! (Tedarikçi Silindi)", "LoginOtp method satır: 74", false);
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
            if(kullaniciOtpValueItem == null)
            {
                await AddKullaniciLoginLog($"id: {id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 75", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            TimeSpan difference = DateTime.Now.ToUniversalTime() - kullaniciOtpValueItem.KayitTarihi.ToUniversalTime();
            if(difference.TotalSeconds > 30)
            {
                await AddKullaniciLoginLog($"id: {id}", "Talep zaman aşımına uğradı!", "GetOtpBarcode method satır: 82", false);
                return NotFound("Talep zaman aşımına uğradı!");
            }

            var userItem = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (userItem == null)
            {
                await AddKullaniciLoginLog($"id: {id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 89", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(userItem.Result.Eposta);
            var twoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = twoFacAuth.GenerateSetupCode("tedarikci.thahomelurztn.com", userItem.Result.Eposta, ConvertSecretToBytes(userUniqueKey, false), 300);

            var response = new ResponseViewModel<TwoFactorAuthSetupResponseDto>
            {
                Success = true,
                Result = new TwoFactorAuthSetupResponseDto
                {
                    BarcodeImageUrl = userItem.Result.IsOtpEnabled ? string.Empty : setupInfo.QrCodeSetupImageUrl,
                    SetupCode = setupInfo.ManualEntryKey
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
                await AddKullaniciLoginLog($"validateOtpCodeRequest.Id: {validateOtpCodeRequest.Id}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 117", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            var kullanici = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (kullanici == null)
            {
                await AddKullaniciLoginLog($"kullaniciOtpValueItem.KullaniciId: {kullaniciOtpValueItem.KullaniciId}", "Kullanıcı bulunamadı!", "GetOtpBarcode method satır: 124", false);
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(kullanici.Result.Eposta);
            int validationWindow = 1;

            var TwoFacAuth = new TwoFactorAuthenticator();
            bool isValid = TwoFacAuth.ValidateTwoFactorPIN(userUniqueKey, validateOtpCodeRequest.Code, validationWindow);
            if (!isValid)
            {
                await AddKullaniciLoginLog(kullanici.Result.Eposta, "Kod hatalı!", "GetOtpBarcode method satır: 135", false);
                return NotFound("Kod hatalı!");
            }

            await _kullanicilarService.SetOtpEnabled(kullanici.Result.Id);

            var kullaniciYetkileri = await _kullaniciYetkileriService.GetByKullaniciId(kullanici.Result.Id);

            var token = JwtTokenGenerator.GenerateToken(kullanici.Result, kullaniciYetkileri.Result, _jwtTokenSettings);

            if (kullanici.Result.IpLoggingEnabled)
            {
                await AddKullaniciLoginLog(kullanici.Result.Eposta, "", "", true);
            }

            return Ok(token);
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        private string GetUserUniqueKey(string email)
        {
            string googleAuthKey = _otpSettings.AuthSecretKey;
            return $"{email}{googleAuthKey}";
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
    }
}
