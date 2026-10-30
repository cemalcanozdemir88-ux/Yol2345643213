using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Core.DTOs.TedarikciMutabakatlari;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class TedarikciMutabakatController : BaseController
    {
        private readonly ITedarikciMutabakatService _tedarikciMutabakatService;

        public TedarikciMutabakatController(ITedarikciMutabakatService tedarikciMutabakatService)
        {
            _tedarikciMutabakatService = tedarikciMutabakatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? tedarikciId, string mutabakatTarihi)
        {
            var success = DateOnly.TryParseExact(
                mutabakatTarihi,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMutabakatTarihi
            );

            var items = await _tedarikciMutabakatService.GetAllDtoAsync(tedarikciId, parsedMutabakatTarihi);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(TedarikciMutabakatCalculateDto calculateDto)
        {
            var success = DateOnly.TryParseExact(
                calculateDto.MutabakatTarihi,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMutabakatTarihi
            );

            var result = await _tedarikciMutabakatService.Calculate(calculateDto.TedarikciId, calculateDto.Devir, parsedMutabakatTarihi);
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
