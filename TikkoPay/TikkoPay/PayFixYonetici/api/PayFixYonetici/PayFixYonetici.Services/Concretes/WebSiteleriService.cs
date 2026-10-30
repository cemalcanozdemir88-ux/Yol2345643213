using AutoMapper;
using PayFixYonetici.Core.DTOs;
using PayFixYonetici.Core.DTOs.WebSiteleri;
using PayFixYonetici.Core.Interfaces;
using PayFixYonetici.Core.Models;
using PayFixYonetici.Services.Interfaces;

namespace PayFixYonetici.Services.Concretes
{
    public class WebSiteleriService : IWebSiteleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSiteleriRepository _webSiteleriRepository;

        public WebSiteleriService(IMapper mapper, IUnitOfWork unitOfWork, IWebSiteleriRepository webSiteleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _webSiteleriRepository = webSiteleriRepository;
        }

        public ResponseViewModel<WebSiteleriDto> GetById(Guid id)
        {
            var item = _webSiteleriRepository.Where(k => k.Id == id && k.SilindiMi == false).FirstOrDefault();
            if (item == null)
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                var result = _mapper.Map<WebSiteleriDto>(item);

                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = true,
                    Result = result
                };
            }
        }

        public async Task<ResponseViewModel<List<WebSiteleriSelectListDto>>> GetSelectListAsync(Guid? webSiteId)
        {
            var items = await _webSiteleriRepository.GetSelectListAsync(webSiteId);
            return new ResponseViewModel<List<WebSiteleriSelectListDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<List<WebSiteleriSelectListDto>>> GetSelectListByAffiliateAsync(Guid affiliateId)
        {
            var items = await _webSiteleriRepository.GetSelectLisByAffiliateAsync(affiliateId);
            return new ResponseViewModel<List<WebSiteleriSelectListDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<List<WebSiteleriListDto>>> GetAllAsync(Guid? webSiteId)
        {
            var items = await _webSiteleriRepository.GetAllAsync(webSiteId);
            return new ResponseViewModel<List<WebSiteleriListDto>>
            {
                Success = true,
                Result = items
            };
        }

        public async Task<ResponseViewModel<List<WebSiteBakiyeListDto>>> GetBakiyeList(Guid? webSiteId)
        {
            var bakiyeList = await _webSiteleriRepository.GetBakiyeList(webSiteId);
            return new ResponseViewModel<List<WebSiteBakiyeListDto>>
            {
                Success = true,
                Result = bakiyeList
            };
        }

        public async Task<ResponseViewModel<WebSiteleriDto>> Delete(Guid id)
        {
            var item = await _webSiteleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<WebSiteleriDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<ResponseViewModel<WebSiteleriDto>> Create(WebSiteleriCreateDto item)
        {
            var model = _mapper.Map<WebSiteleri>(item);

            await _webSiteleriRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<WebSiteleriDto>> Update(WebSiteleriUpdateDto itemDto)
        {
            var dbItem = await _webSiteleriRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<WebSiteleriDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _webSiteleriRepository.Update(dbItem, itemDto);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<WebSiteleriDto>
            {
                Success = true
            };
        }










    }
}
