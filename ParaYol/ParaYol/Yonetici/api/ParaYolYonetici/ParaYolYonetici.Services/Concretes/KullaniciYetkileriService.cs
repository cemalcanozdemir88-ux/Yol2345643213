using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.KullaniciYetkileri;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class KullaniciYetkileriService : IKullaniciYetkileriService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IKullaniciYetkileriRepository _repository;

        public KullaniciYetkileriService(IMapper mapper, IKullaniciYetkileriRepository repository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseViewModel<KullaniciYetkileriListDto>> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullaniciYetkileriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KullaniciYetkileriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KullaniciYetkileriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<KullaniciYetkileriListDto>>> GetByKullaniciId(Guid kullaniciId)
        {
            var items = await _repository.GetByKullaniciId(kullaniciId);

            return new ResponseViewModel<List<KullaniciYetkileriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModel<KullaniciYetkileriListDto>> Create(KullaniciYetkileriCreateDto item)
        {
            var kullaniciYetkileri = _mapper.Map<KullaniciYetkileri>(item);

            await _repository.AddAsync(kullaniciYetkileri);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<KullaniciYetkileriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KullaniciYetkileriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KullaniciYetkileriListDto>> Update(KullaniciYetkileriUpdateDto item)
        {
            var yetkiItem = await _repository.GetByIdAsync(item.Id);
            if (yetkiItem == null)
            {
                return new ResponseViewModel<KullaniciYetkileriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _repository.Update(yetkiItem, item);

            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<KullaniciYetkileriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KullaniciYetkileriListDto>> Delete(KullaniciYetkileriDeleteDto item)
        {
            var yetkiItem = _repository.Find(k => k.KullaniciId == item.KullaniciId && k.YetkiId == item.YetkiId).ToList();
            
            foreach (var record in yetkiItem)
            {
                record.SilindiMi = true;
            }

            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullaniciYetkileriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
