using AutoMapper;
using ParaYolYonetici.Core;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.HavaleBankalari;
using ParaYolYonetici.Core.DTOs.Tedarikciler;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class TedarikciService : ITedarikciService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITedarikciRepository _tedarikciRepository;
        private readonly ITelegramBotService _telegramBotService;

        public TedarikciService(IUnitOfWork unitOfWork, IMapper mapper, ITedarikciRepository tedarikciRepository, ITelegramBotService telegramBotService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tedarikciRepository = tedarikciRepository;
            _telegramBotService = telegramBotService;
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

        public async Task<ResponseViewModel<List<TedarikcilerListDto>>> GetAllWithPaging(bool? bankaHavale, bool? otoPuan, bool? durum, int pageNumber, int recordsPerPage)
        {
            var count = await _tedarikciRepository.GetCountAsync(bankaHavale, otoPuan, durum);
            var items = await _tedarikciRepository.GetAllDtoAsync(bankaHavale, otoPuan, durum, pageNumber, recordsPerPage);


            var totalPages = Convert.ToInt32(Math.Ceiling((double)count / (double)recordsPerPage));

            return new ResponseViewModelPagination<List<TedarikcilerListDto>>
            {
                Success = true,
                Result = items.ToList(),
                TotalPages = totalPages,
                TotalRecords = count
            };
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

        public async Task<ResponseViewModel<List<TedarikcilerSelectList>>> GetSelectList()
        {
            var tedarikciler = await _tedarikciRepository.GetSelectListAsync();
            return new ResponseViewModel<List<TedarikcilerSelectList>>
            {
                Success = true,
                Result = _mapper.Map<List<TedarikcilerSelectList>>(tedarikciler)
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

        public async Task<ResponseViewModel<List<TedarikciBakiyeListDto>>> GetBakiyeList(Guid? tedarikciId, int? tedarikciDurum)
        {
            var bakiyeList = await _tedarikciRepository.GetBakiyeList(tedarikciId, tedarikciDurum);
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

            if (dbItem.YatirimPuani != itemDto.YatirimPuani)
            {
                var telegramMessage = $"{dbItem.Unvan} tedarikçisinin yatırım puanı değişti. Eski Yatırım Puanı: {dbItem.YatirimPuani} Yeni Yatırım Puanı: {itemDto.YatirimPuani}";
                var telegramResponse = await _telegramBotService.SendMessageAsync(Constants.PuanlarChannelId, telegramMessage);
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
