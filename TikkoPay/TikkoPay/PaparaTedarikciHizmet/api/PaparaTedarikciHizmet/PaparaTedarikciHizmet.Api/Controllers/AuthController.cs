using AutoMapper;
using Google.Authenticator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using PaparaTedarikciHizmet.Api.Infrastructure;
using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.Auth;
using PaparaTedarikciHizmet.Core.DTOs.Kullanicilar;
using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Core.Models;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Api.Controllers
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

        public AuthController(IOptions<JwtTokenSettings> jwtTokenSettingsOptions,
                              IKullanicilarService kullanicilarService,
                              IKullaniciYetkileriService kullaniciYetkileriService,
                              IOptions<OtpSettings> otpSettingsOptions,
                              IKullaniciOtpValuesRepository kullaniciOtpValuesRepository,
                              IMapper mapper)
        {
            _jwtTokenSettings = jwtTokenSettingsOptions.Value;
            _kullanicilarService = kullanicilarService;
            _kullaniciYetkileriService = kullaniciYetkileriService;
            _otpSettings = otpSettingsOptions.Value;
            _kullaniciOtpValuesRepository = kullaniciOtpValuesRepository;
            _mapper = mapper;
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
            if(kullaniciOtpValueItem == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            TimeSpan difference = DateTime.Now.ToUniversalTime() - kullaniciOtpValueItem.KayitTarihi.ToUniversalTime();
            if(difference.TotalSeconds > 30)
            {
                return NotFound("Talep zaman aşımına uğradı!");
            }

            var userItem = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (userItem == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(userItem.Result.Eposta);
            var twoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = twoFacAuth.GenerateSetupCode("papara-tedarikci.thyhometursss.com", userItem.Result.Eposta, ConvertSecretToBytes(userUniqueKey, false), 300);

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
                return NotFound("Kullanıcı bulunamadı!");
            }

            var kullanici = await _kullanicilarService.GetById(kullaniciOtpValueItem.KullaniciId);
            if (kullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı!");
            }

            string userUniqueKey = GetUserUniqueKey(kullanici.Result.Eposta);
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

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);

        private string GetUserUniqueKey(string email)
        {
            string googleAuthKey = _otpSettings.AuthSecretKey;
            return $"{email}{googleAuthKey}";
        }
    }
}
