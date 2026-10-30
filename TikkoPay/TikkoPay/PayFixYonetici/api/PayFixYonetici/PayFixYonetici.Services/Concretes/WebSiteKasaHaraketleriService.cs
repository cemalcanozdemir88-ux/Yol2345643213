using AutoMapper;
using PayFixYonetici.Core.DTOs;
using PayFixYonetici.Core.DTOs.WebSiteKasaHaraketleri;
using PayFixYonetici.Core.Interfaces;
using PayFixYonetici.Core.Models;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Services.Concretes
{
    public class WebSiteKasaHaraketleriService : IWebSiteKasaHaraketleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteKasaHaraketleriRepository _webSiteKasaHaraketleriRepository;

        public WebSiteKasaHaraketleriService(IMapper mapper, IUnitOfWork unitOfWork, IWebSiteKasaHaraketleriRepository webSiteKasaHaraketleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteKasaHaraketleriRepository = webSiteKasaHaraketleriRepository;
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> GetById(Guid id)
        {
            var item = await _webSiteKasaHaraketleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<WebSiteKasaHaraketleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<WebSiteKasaHaraketleriListDto>>> GetAll(Guid? webSiteId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _webSiteKasaHaraketleriRepository.GetAllDtoAsync(webSiteId, durum, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<WebSiteKasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }


        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Create(WebSiteKasaHaraketleriCreateDto item)
        {
            WebSiteKasaHaraketleri webSiteKasaHaraketleri = _mapper.Map<WebSiteKasaHaraketleri>(item);
            await _webSiteKasaHaraketleriRepository.AddAsync(webSiteKasaHaraketleri);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Update(WebSiteKasaHaraketleriUpdateDto item)
        {
            var dbItem = await _webSiteKasaHaraketleriRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _webSiteKasaHaraketleriRepository.Update(dbItem, item);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<WebSiteKasaHaraketleriListDto>> Delete(Guid id)
        {
            var item = await _webSiteKasaHaraketleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteKasaHaraketleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

    }
}
