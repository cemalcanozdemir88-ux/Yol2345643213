using AutoMapper;
using PayCoYonetici.Core.DTOs.Affiliate;
using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Core.Models;
using PayCoYonetici.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayCoYonetici.Services.Concretes
{
    public class AffiliateService : IAffiliateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAffiliateRepository _affiliateRepository;

        public AffiliateService(IMapper mapper, IUnitOfWork unitOfWork, IAffiliateRepository affiliateRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _affiliateRepository = affiliateRepository;
        }

        public ResponseViewModel<AffiliateDto> GetById(Guid id)
        {
            var item = _affiliateRepository.Where(k => k.Id == id && k.SilindiMi == false).FirstOrDefault();
            if (item == null)
            {
                return new ResponseViewModel<AffiliateDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                var result = _mapper.Map<AffiliateDto>(item);

                return new ResponseViewModel<AffiliateDto>
                {
                    Success = true,
                    Result = result
                };
            }
        }

        public async Task<ResponseViewModel<List<AffiliateBakiyeListDto>>> GetBakiyeList(Guid? affiliateId, Guid? websiteId)
        {
            var bakiyeList = await _affiliateRepository.GetBakiyeList(affiliateId, websiteId);
            return new ResponseViewModel<List<AffiliateBakiyeListDto>>
            {
                Success = true,
                Result = bakiyeList
            };
        }

        public async Task<ResponseViewModel<List<AffiliateDto>>> GetAllAsync(Guid? affiliateId)
        {
            var affilaites = await _affiliateRepository.GetAllAsync(affiliateId);
            return new ResponseViewModel<List<AffiliateDto>>
            {
                Success = true,
                Result = _mapper.Map<List<AffiliateDto>>(affilaites)
            };
        }

        public async Task<ResponseViewModel<AffiliateDto>> Delete(Guid id)
        {
            var item = await _affiliateRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<AffiliateDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<AffiliateDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };

        }

        public async Task<ResponseViewModel<AffiliateDto>> Create(AffiliateCreateDto item)
        {
            Affiliates model = _mapper.Map<Affiliates>(item);

            await _affiliateRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<AffiliateDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<AffiliateDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<AffiliateDto>> Update(AffiliateUpdateDto itemDto)
        {
            var dbItem = await _affiliateRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<AffiliateDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _affiliateRepository.Update(dbItem, itemDto);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<AffiliateDto>
            {
                Success = true
            };
        }
    }
}
