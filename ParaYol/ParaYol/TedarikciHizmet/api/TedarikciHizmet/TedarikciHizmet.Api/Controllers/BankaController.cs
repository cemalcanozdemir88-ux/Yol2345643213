using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
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
            var bankalar = await _bankaService.GetAll();
            return Ok(bankalar);
        }
    }
}
