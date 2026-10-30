using AutoMapper;
using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.YatirimTalepleri;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Services.Interfaces;

namespace PayCoYonetici.Services.Concretes
{
    public class YatirimTalepleriService : IYatirimTalepleriService
    {
        private readonly IMapper _mapper;
        private readonly IYatirimTalepleriRepository _yatirimTalepleriRepository;

        public YatirimTalepleriService(IMapper mapper,
                                       IYatirimTalepleriRepository yatirimTalepleriRepository)
        {
            _mapper = mapper;
            _yatirimTalepleriRepository = yatirimTalepleriRepository;
        }

        public async Task<ResponseViewModel<YatirimTalepleriListDto>> GetById(Guid id)
        {
            var item = await _yatirimTalepleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<YatirimTalepleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<YatirimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriRepository.GetAllAsync(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriListDto>>> GetAll(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllAsync(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>> GetAllForWebSite(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 20)
        {
            var count = await _yatirimTalepleriRepository.GetCountAsync(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriRepository.GetAllForWebSiteAsync(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriBaseListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<ToplamYatirimTutariDto>> GetToplamYatirimTutari(Guid? webSiteId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var result = await _yatirimTalepleriRepository.GetToplamYatirimTutari(webSiteId, searchText, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<ToplamYatirimTutariDto> { Success = true, Result = result };
        }
    }
}
