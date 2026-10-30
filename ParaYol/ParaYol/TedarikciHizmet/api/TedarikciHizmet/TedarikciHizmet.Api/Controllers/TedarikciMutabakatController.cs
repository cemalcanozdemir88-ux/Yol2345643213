using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Api.Controllers
{
    public class TedarikciMutabakatController : BaseController
    {
        private readonly ITedarikciMutabakatService _tedarikciMutabakatService;

        public TedarikciMutabakatController(ITedarikciMutabakatService tedarikciMutabakatService)
        {
            _tedarikciMutabakatService = tedarikciMutabakatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string mutabakatTarihi)
        {
            var success = DateOnly.TryParseExact(
                mutabakatTarihi,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedMutabakatTarihi
            );

            var tedarikciId = GetTedarikciId();

            var items = await _tedarikciMutabakatService.GetAllDtoAsync(tedarikciId, parsedMutabakatTarihi);
            return Ok(items);
        }
    }
}
