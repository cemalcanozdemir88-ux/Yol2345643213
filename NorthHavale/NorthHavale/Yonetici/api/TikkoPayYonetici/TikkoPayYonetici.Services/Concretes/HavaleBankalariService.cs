using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.HavaleBankalari;
using TikkoPayYonetici.Core.Enums;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class HavaleBankalariService : IHavaleBankalariService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;

        public HavaleBankalariService(IMapper mapper, IUnitOfWork unitOfWork, ITedarikciRepository tedarikciRepository, IHavaleBankalariRepository havaleBankalariRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tedarikciRepository = tedarikciRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> GetById(Guid id)
        {
            var item = await _havaleBankalariRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = true,
                    Result = _mapper.Map<HavaleBankalariListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<HavaleBankalariListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter)
        {
            var items = await _havaleBankalariRepository.GetAllDtoAsync(tedarikciId, bankaId, searchText, havaleBankalariDurumFilter);
            return new ResponseViewModel<List<HavaleBankalariListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<HavaleBankalariListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage)
        {
            var count = await _havaleBankalariRepository.GetCountAsync(tedarikciId, bankaId, searchText, havaleBankalariDurumFilter);
            var items = await _havaleBankalariRepository.GetAllDtoAsync(tedarikciId, bankaId, searchText, havaleBankalariDurumFilter, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<HavaleBankalariListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Create(HavaleBankalariCreateDto item)
        {
            HavaleBankalari havaleBankalari = _mapper.Map<HavaleBankalari>(item);
            await _havaleBankalariRepository.AddAsync(havaleBankalari);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Update(HavaleBankalariUpdateDto item)
        {
            var dbItem = await _havaleBankalariRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _havaleBankalariRepository.Update(dbItem, item);
            var result = await _unitOfWork.SaveAsync();
            
            return new ResponseViewModel<HavaleBankalariListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<HavaleBankalariListDto>> Delete(Guid id)
        {
            var item = await _havaleBankalariRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<HavaleBankalariListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<HavaleBankalariListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
