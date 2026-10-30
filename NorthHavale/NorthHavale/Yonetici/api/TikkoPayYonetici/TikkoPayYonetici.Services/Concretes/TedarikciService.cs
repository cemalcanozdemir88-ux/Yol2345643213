using AutoMapper;
using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.Tedarikciler;
using TikkoPayYonetici.Core.Interfaces;
using TikkoPayYonetici.Core.Models;
using TikkoPayYonetici.Services.Interfaces;

namespace TikkoPayYonetici.Services.Concretes
{
    public class TedarikciService : ITedarikciService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITedarikciRepository _tedarikciRepository;

        public TedarikciService(IUnitOfWork unitOfWork, IMapper mapper, ITedarikciRepository tedarikciRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tedarikciRepository = tedarikciRepository;
        }

        public ResponseViewModel<TedarikcilerDto> GetById(Guid id)
        {
            var tedarikci = _tedarikciRepository.Where(k => k.Id == id && k.SilindiMi == false).FirstOrDefault();
            if (tedarikci == null)
            {
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                var result = _mapper.Map<TedarikcilerDto>(tedarikci);
                
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = true,
                    Result = result
                };
            }
        }

        public async Task<ResponseViewModel<List<TedarikcilerListDto>>> GetAll()
        {
            var tedarikciler = (await _tedarikciRepository.GetAllAsync()).OrderBy(k => k.Unvan);
            return new ResponseViewModel<List<TedarikcilerListDto>>
            {
                Success = true,
                Result = _mapper.Map<List<TedarikcilerListDto>>(tedarikciler)
            };
        }

        public async Task<ResponseViewModel<List<TedarikcilerSelectListWithBakiyeDto>>> GetSelectListWithBakiye()
        {
            var tedarikciler = await _tedarikciRepository.GetSelectListWithBakiyeAsync();
            return new ResponseViewModel<List<TedarikcilerSelectListWithBakiyeDto>>
            {
                Success = true,
                Result = _mapper.Map<List<TedarikcilerSelectListWithBakiyeDto>>(tedarikciler)
            };
        }

        public async Task<ResponseViewModel<List<TedarikciBakiyeListDto>>> GetBakiyeList(Guid? tedarikciId)
        {
            var bakiyeList = await _tedarikciRepository.GetBakiyeList(tedarikciId);
            return new ResponseViewModel<List<TedarikciBakiyeListDto>>
            {
                Success = true,
                Result = bakiyeList
            };
        }

        public async Task<ResponseViewModel<TedarikcilerDto>> Delete(Guid id)
        {
            var item = await _tedarikciRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<TedarikcilerDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };

        }

        public async Task<ResponseViewModel<TedarikcilerDto>> Create(TedarikcilerCreateDto item)
        {
            Tedarikciler model = _mapper.Map<Tedarikciler>(item);

            await _tedarikciRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<TedarikcilerDto>> Update(TedarikcilerUpdateDto itemDto)
        {
            var dbItem = await _tedarikciRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<TedarikcilerDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _tedarikciRepository.Update(dbItem, itemDto);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<TedarikcilerDto>
            {
                Success = true
            };
        }
    }
}
