using Microsoft.AspNetCore.Mvc;
using ParaYolYonetici.Core.DTOs.Affiliate;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Api.Controllers
{
    public class AffiliatesController : BaseController
    {
        private readonly IAffiliateService _affiliateService;

        public AffiliatesController(IAffiliateService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var item = _affiliateService.GetById(id);
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
            Guid? affiliateId = null;
            if (!IsSuperAdmin())
            {
                affiliateId = GetAffiliateId();
            }

            var affiliate = await _affiliateService.GetAllAsync(affiliateId);
            return Ok(affiliate);
        }

        [HttpGet("get-bakiyelist")]
        public async Task<IActionResult> GetBakiyeList([FromQuery] Guid? affiliateId, Guid? websiteId)
        {
            if (!IsSuperAdmin())
            {
                affiliateId = GetAffiliateId();
            }

            var tedarikciler = await _affiliateService.GetBakiyeList(affiliateId, websiteId);
            return Ok(tedarikciler);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AffiliateCreateDto affiliateDto)
        {
            var result = await _affiliateService.Create(affiliateDto);
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
        public async Task<IActionResult> Update(AffiliateUpdateDto affiliateDto)
        {
            var result = await _affiliateService.Update(affiliateDto);
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
            var result = await _affiliateService.Delete(id);
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
