using AutoMapper;
using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.KasaHaraketleri;
using PaparaTedarikciHizmet.Core.Interfaces;
using PaparaTedarikciHizmet.Core.Models;
using PaparaTedarikciHizmet.Services.Interfaces;

namespace PaparaTedarikciHizmet.Services.Concretes
{
    public class KasaHaraketleriService : IKasaHaraketleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKasaHaraketleriRepository _kasaHaraketleriRepository;

        public KasaHaraketleriService(IMapper mapper, IUnitOfWork unitOfWork, IKasaHaraketleriRepository kasaHaraketleriRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _kasaHaraketleriRepository = kasaHaraketleriRepository;
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> GetById(Guid id)
        {
            var item = await _kasaHaraketleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KasaHaraketleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd)
        {
            var items = await _kasaHaraketleriRepository.GetAllDtoAsync(tedarikciId, durum, createdDateStart, createdDateEnd);

            return new ResponseViewModel<List<KasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModelPagination<List<KasaHaraketleriListDto>>> GetAll(Guid? tedarikciId, int? durum, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber = 1, int recordsPerPage = 10)
        {
            var count = await _kasaHaraketleriRepository.GetCountAsync(tedarikciId, durum, createdDateStart, createdDateEnd);
            var items = await _kasaHaraketleriRepository.GetAllDtoAsync(tedarikciId, durum, createdDateStart, createdDateEnd, pageNumber, recordsPerPage);

            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<KasaHaraketleriListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Create(KasaHaraketleriCreateDto item)
        {
            KasaHaraketleri kasaHaraketleri = _mapper.Map<KasaHaraketleri>(item);
            await _kasaHaraketleriRepository.AddAsync(kasaHaraketleri);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Update(KasaHaraketleriUpdateDto item)
        {
            var dbItem = await _kasaHaraketleriRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _kasaHaraketleriRepository.Update(dbItem, item);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<KasaHaraketleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KasaHaraketleriListDto>> Delete(Guid id)
        {
            var item = await _kasaHaraketleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KasaHaraketleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            _kasaHaraketleriRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KasaHaraketleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

        public async Task<decimal> GetToplamKasaHaraketiGiris(Guid tedarikciId)
        {
            return await _kasaHaraketleriRepository.GetToplamKasaHaraketiGiris(tedarikciId);
        }

        public async Task<decimal> GetToplamKasaHaraketiCikis(Guid tedarikciId)
        {
            return await _kasaHaraketleriRepository.GetToplamKasaHaraketiCikis(tedarikciId);
        }
    }
}
