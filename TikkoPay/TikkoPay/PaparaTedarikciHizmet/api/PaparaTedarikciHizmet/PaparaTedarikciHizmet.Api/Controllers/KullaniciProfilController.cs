using Microsoft.AspNetCore.Mvc;
using PaparaTedarikciHizmet.Core.DTOs.Kullanicilar;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Api.Controllers
{
    public class KullaniciProfilController : BaseController
    {
        private readonly IKullanicilarService _kullanicilarService;

        public KullaniciProfilController(IKullanicilarService kullanicilarService)
        {
            _kullanicilarService = kullanicilarService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = await _kullanicilarService.ChangePassword(LoggedInUserId, changePasswordDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
