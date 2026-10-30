using AutoMapper;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.Banka;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;
using System.Net.Http;
using System.Security.Claims;

namespace TedarikciHizmet.Services.Concretes
{
    public class BankaService : IBankaService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IBankalarRepository _bankalarRepository;

        public BankaService(IUnitOfWork unitOfWork, IMapper mapper, IBankalarRepository bankalarRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bankalarRepository = bankalarRepository;
        }

        public async Task<ResponseViewModel<BankaListDto>> GetById(Guid id)
        {
            var banka = await _bankalarRepository.GetByIdAsync(id);
            if (banka == null)
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = true,
                    Result = _mapper.Map<BankaListDto>(banka)
                };
            }
        }

        public async Task<ResponseViewModel<List<BankaListDto>>> GetAll()
        {
            var bankalar = (await _bankalarRepository.GetAllAsync()).OrderBy(k => k.Baslik);
            return new ResponseViewModel<List<BankaListDto>>
            {
                Success = true,
                Result = _mapper.Map<List<BankaListDto>>(bankalar)
            };
        }

        public async Task<ResponseViewModel<BankaListDto>> Create(BankaCreateDto item)
        {
            Bankalar banka = _mapper.Map<Bankalar>(item);

            await _bankalarRepository.AddAsync(banka);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<BankaListDto>> Update(BankaUpdateDto item)
        {
            var bankaItem = await _bankalarRepository.GetByIdAsync(item.Id);
            if (bankaItem == null)
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _bankalarRepository.Update(bankaItem, item);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<BankaListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<BankaListDto>> Delete(Guid id)
        {
            var item = await _bankalarRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<BankaListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            _bankalarRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<BankaListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

    }
}
