using Microsoft.AspNetCore.Mvc;
using PaparaYonetici.Core.DTOs.Banka;
using PaparaYonetici.Services.Interfaces;

namespace PaparaYonetici.Api.Controllers
{
    public class BankaController : BaseController
    {
        public readonly IBankaService _bankaService;

        public BankaController(IBankaService bankaService)
        {
            _bankaService = bankaService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _bankaService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var username = User.FindFirst(ClaimTypes.Name)?.Value;
            //var userId = User.FindFirst("nameidentifier")?.Value;
            //var role = User.FindFirst(ClaimTypes.Role)?.Value;


            //var identity = HttpContext.User.Identity as ClaimsIdentity;
            //if (identity != null)
            //{
            //    var userClaims = identity.Claims;

            //    var name = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            //    var username = userClaims.FirstOrDefault(x => x.Type == "nameidentifier")?.Value;
            //    var role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            //}

            var bankalar = await _bankaService.GetAll();
            return Ok(bankalar);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BankaCreateDto banka)
        {
            var result = await _bankaService.Create(banka);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(BankaUpdateDto banka)
        { 

            var result = await _bankaService.Update(banka);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _bankaService.Delete(id);
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
