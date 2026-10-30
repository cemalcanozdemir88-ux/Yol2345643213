using Microsoft.AspNetCore.Mvc;
using TikkoPayYonetici.Core.DTOs.Banka;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Api.Controllers
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
        [ResponseCache(Duration = 900)]
        public async Task<IActionResult> GetAll()
        {
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
