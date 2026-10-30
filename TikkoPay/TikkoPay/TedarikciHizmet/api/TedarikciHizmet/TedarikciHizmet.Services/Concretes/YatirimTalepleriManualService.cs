using AutoMapper;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class YatirimTalepleriManualService : IYatirimTalepleriManualService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IYatirimCekimTalepleriManualRepository _yatirimTalepleriManualRepository;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly IWebSiteleriRepository _webSiteleriRepository;
        private readonly IHavaleBankalariRepository _havaleBankalariRepository;
        private readonly IWebSitePaymentMethodsRepository _webSitePaymentMethodsRepository;

        public YatirimTalepleriManualService(IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IYatirimCekimTalepleriManualRepository yatirimTalepleriManualRepository, 
            ITedarikciRepository tedarikciRepository, 
            IWebSiteleriRepository webSiteleriRepository, 
            IHavaleBankalariRepository havaleBankalariRepository, 
            IWebSitePaymentMethodsRepository webSitePaymentMethodsRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _yatirimTalepleriManualRepository = yatirimTalepleriManualRepository;
            _tedarikciRepository = tedarikciRepository;
            _webSiteleriRepository = webSiteleriRepository;
            _havaleBankalariRepository = havaleBankalariRepository;
            _webSitePaymentMethodsRepository = webSitePaymentMethodsRepository;
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> ApproveAsync(Guid id, Guid tedarikciId)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id, tedarikciId);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            yatirimTalepleriDbItem.Durum = Durum.Onaylandi;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Success = true,
                Message = "Yatırım talebi onaylandı"
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> CancelAsync(Guid id, Guid tedarikciId, string iptalAciklama)
        {
            var yatirimTalepleriDbItem = GetYatirimTalepleri(id, tedarikciId);
            if (yatirimTalepleriDbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            yatirimTalepleriDbItem.IptalAciklama = iptalAciklama;
            yatirimTalepleriDbItem.Durum = Durum.Red;
            yatirimTalepleriDbItem.OnayTarihi = DateTime.Now;

            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Success = true,
                Message = "Yatırım talebi iptal edildi",
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> DeleteAsync(Guid id)
        {
            var item = await _yatirimTalepleriManualRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            _yatirimTalepleriManualRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<ResponseViewModel<List<YatirimTalepleriManualListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _yatirimTalepleriManualRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            return new ResponseViewModel<List<YatirimTalepleriManualListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<YatirimTalepleriManualListDto>>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage)
        {
            var count = await _yatirimTalepleriManualRepository.GetCountAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd);
            var items = await _yatirimTalepleriManualRepository.GetAllAsync(tedarikciId, bankaId, searchText, IslemTip.Deposit, durum, approveDateStart, approveDateEnd, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<YatirimTalepleriManualListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> GetByIdAsync(Guid id)
        {
            var item = await _yatirimTalepleriManualRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = true,
                    Result = _mapper.Map<YatirimTalepleriManualListDto>(item)
                };
            }
        }

        public async Task<int> GetToplamBekleyenYatirimlar(Guid? tedarikciId)
        {
            var count = await _yatirimTalepleriManualRepository.GetCountAsync(tedarikciId, IslemTip.Deposit, Durum.Bekliyor);
            return count;
        }

        public async Task<ResponseViewModel<YatirimTalepleriManualListDto>> UpdateAsync(YatirimTalepleriManualUpdateDto itemDto)
        {
            var dbItem = await _yatirimTalepleriManualRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<YatirimTalepleriManualListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _yatirimTalepleriManualRepository.Update(dbItem, itemDto);
            _ = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<YatirimTalepleriManualListDto>
            {
                Success = true
            };
        }

        private YatirimCekimTalepleriManual GetYatirimTalepleri(Guid id, Guid tedarikciId)
        {
            return _yatirimTalepleriManualRepository.Find(k => k.Id == id &&
                                                         k.TedarikciId == tedarikciId &&
                                                         k.IslemTipi == IslemTip.Deposit &&
                                                         k.Durum == Durum.Bekliyor &&
                                                         !k.SilindiMi).FirstOrDefault();
        }
    }
}
