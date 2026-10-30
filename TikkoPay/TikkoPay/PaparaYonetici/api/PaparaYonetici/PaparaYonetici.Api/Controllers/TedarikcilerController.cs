using PaparaYonetici.Core.DTOs.Tedarikciler;
using PaparaYonetici.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PaparaYonetici.Api.Controllers
{
    public class TedarikcilerController : BaseController
    {
        public readonly ITedarikciService _tedarikciService;

        public TedarikcilerController(ITedarikciService tedarikciService)
        {
            _tedarikciService = tedarikciService;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var item = _tedarikciService.GetById(id);
            if (item.Success)
            {
                return Ok(item);
            }
            else
            {
                return BadRequest(item);
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var tedarikciler = await _tedarikciService.GetAll();
            return Ok(tedarikciler);
        }

        [HttpGet("get-bakiyelist")]
        public async Task<IActionResult> GetBakiyeList([FromQuery]Guid? tedarikciId)
        {
            var tedarikciler = await _tedarikciService.GetBakiyeList(tedarikciId);
            return Ok(tedarikciler);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TedarikcilerCreateDto tedarikcilerDto)
        {  
            var result = await _tedarikciService.Create(tedarikcilerDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> Update(TedarikcilerUpdateDto tedarikcilerDto)
        {
            var result = await _tedarikciService.Update(tedarikcilerDto);
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
            var result = await _tedarikciService.Delete(id);
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
