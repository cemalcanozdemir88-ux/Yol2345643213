using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.KullaniciBlackList;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class KullaniciBlackListsService : IKullaniciBlackListsService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKullaniciBlackListsRepository _kullaniciBlackListsRepository;

        public KullaniciBlackListsService(IMapper mapper, IUnitOfWork unitOfWork, IKullaniciBlackListsRepository kullaniciBlackListsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _kullaniciBlackListsRepository = kullaniciBlackListsRepository;
        }

        public async Task<ResponseViewModelPagination<List<KullaniciBlackListDto>>> GetAll(Guid? websiteId, long? userId, int pageNumber, int recordsPerPage)
        {
            var count = await _kullaniciBlackListsRepository.GetCountAsync(websiteId, userId);
            var items = await _kullaniciBlackListsRepository.GetAllDtoAsync(websiteId, userId, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<KullaniciBlackListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<KullaniciBlackListDto>> Create(KullaniciBlackListsCreateDto item)
        {
            KullaniciBlackLists kullaniciBlackList = _mapper.Map<KullaniciBlackLists>(item);
            await _kullaniciBlackListsRepository.AddAsync(kullaniciBlackList);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<KullaniciBlackListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KullaniciBlackListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KullaniciBlackListDto>> Delete(Guid id)
        {
            var item = await _kullaniciBlackListsRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullaniciBlackListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullaniciBlackListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
