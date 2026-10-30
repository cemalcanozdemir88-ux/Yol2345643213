using ParaYolYonetici.Core.DTOs.Tedarikciler;
using ParaYolYonetici.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ParaYolYonetici.Api.Controllers
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

        [HttpGet("get-all-with-paging")]
        public async Task<IActionResult> GetAll([FromQuery] bool? bankaHavale, bool? otoPuan, bool? durum, int pageNumber = 1, int recordsPerPage = 10)
        {
            var tedarikciler = await _tedarikciService.GetAllWithPaging(bankaHavale, otoPuan, durum, pageNumber, recordsPerPage);
            return Ok(tedarikciler);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var tedarikciler = await _tedarikciService.GetAll();
            return Ok(tedarikciler);
        }

        [HttpGet("get-select-list")]
        [ResponseCache(Duration = 900)]
        public async Task<IActionResult> GetSelectList()
        {
            var tedarikciler = await _tedarikciService.GetSelectList();
            return Ok(tedarikciler);
        }

        [HttpGet("get-select-list-with-bakiye")]
        [ResponseCache(Duration = 900)]
        public async Task<IActionResult> GetSelectListWithBakiye()
        {
            var tedarikciler = await _tedarikciService.GetSelectListWithBakiye();
            return Ok(tedarikciler);
        }

        [HttpGet("get-bakiyelist")]
        public async Task<IActionResult> GetBakiyeList([FromQuery]Guid? tedarikciId, int? tedarikciDurum)
        {
            var tedarikciler = await _tedarikciService.GetBakiyeList(tedarikciId, tedarikciDurum);
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
