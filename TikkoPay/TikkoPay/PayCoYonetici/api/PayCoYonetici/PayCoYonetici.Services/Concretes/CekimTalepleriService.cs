using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.CekimTalepleri;
using PayCoYonetici.Core.Enums;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Services.Interfaces;

namespace PayCoYonetici.Services.Concretes
{
    public class CekimTalepleriService : ICekimTalepleriService
    {
        private ICekimTalepleriRepository _cekimTalepleriRepository;

        public CekimTalepleriService(ICekimTalepleriRepository cekimTalepleriRepository)
        {
            _cekimTalepleriRepository = cekimTalepleriRepository;
        }

        public async Task<ResponseViewModelPagination<List<CekimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var count = await _cekimTalepleriRepository.GetCountAsync(webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _cekimTalepleriRepository.GetAllAsync(webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<CekimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, bool durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var count = await _cekimTalepleriRepository.GetCountAsync(webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _cekimTalepleriRepository.GetAllForWebSiteAsync(webSiteId, searchText, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<CekimTalepleriBaseListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<ToplamCekimTutariDto>> GetToplamCekimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _cekimTalepleriRepository.GetToplamCekimTutari(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamCekimTutariDto> { Success = true, Result = result };
        }
    }
}
