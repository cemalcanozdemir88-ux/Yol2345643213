using AutoMapper;
using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.KullaniciIpAdresleri;
using ParaYolYonetici.Core.Interfaces;
using ParaYolYonetici.Core.Models;
using ParaYolYonetici.Services.Interfaces;

namespace ParaYolYonetici.Services.Concretes
{
    public class KullaniciIpAdresleriService : IKullaniciIpAdresleriService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKullaniciIpAdresleriRepository _kullaniciIpAdresleriRepository;
        private readonly IKullanicilarRepository _kullanicilarRepository;

        public KullaniciIpAdresleriService(IMapper mapper,
                                           IUnitOfWork unitOfWork,
                                           IKullaniciIpAdresleriRepository kullaniciIpAdresleriRepository,
                                           IKullanicilarRepository kullanicilarRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _kullaniciIpAdresleriRepository = kullaniciIpAdresleriRepository;
            _kullanicilarRepository = kullanicilarRepository;
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> GetById(Guid id)
        {
            var item = await _kullaniciIpAdresleriRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KullaniciIpAdresleriListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<KullaniciIpAdresleriListDto>>> GetAll(Guid? tedarikciId, string searchText)
        {
            var items = await _kullaniciIpAdresleriRepository.GetAllDtoAsync(tedarikciId, searchText);
            return new ResponseViewModel<List<KullaniciIpAdresleriListDto>>
            {
                Success = true,
                Result = items.ToList()
            };
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriWithAvailableCount>> GetByKullaniciId(Guid kullaniciId)
        {
            var kullaniciItem = await _kullanicilarRepository.GetByIdAsync(kullaniciId);
            var items = await _kullaniciIpAdresleriRepository.GetByKullaniciIdAsync(kullaniciId);

            return new ResponseViewModel<KullaniciIpAdresleriWithAvailableCount>
            {
                Success = true,
                Result = new KullaniciIpAdresleriWithAvailableCount
                {
                    IpAdresSayisi = kullaniciItem.IpAdresSayisi,
                    kullaniciIpAdresleriListDtos = items.Select(k => new KullaniciIpAdresleriListDto
                    {
                        Id = k.Id,
                        KullaniciId = k.KullaniciId,
                        IpAdresi = k.IpAdresi,
                    }).ToList()
                }
            };
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> CreateIfAvaliable(KullaniciIpAdresleriCreateDto item)
        {
            var output = new ResponseViewModel<KullaniciIpAdresleriListDto>
            {
                Success = true
            };

            var kullanici = await _kullanicilarRepository.GetByIdAsync(item.KullaniciId);
            var ipList = await _kullaniciIpAdresleriRepository.GetByKullaniciIdAsync(item.KullaniciId);

            if (ipList.Count >= kullanici.IpAdresSayisi)
            {
                return output;
            }

            if (ipList.Any(k => k.IpAdresi.Equals(item.IpAdresi)))
            {
                return output;
            }

            await Create(item);

            return output;
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Create(KullaniciIpAdresleriCreateDto item)
        {
            KullaniciIpAdresleri kullaniciIpAdresileri = _mapper.Map<KullaniciIpAdresleri>(item);
            await _kullaniciIpAdresleriRepository.AddAsync(kullaniciIpAdresileri);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Update(KullaniciIpAdresleriUpdateDto item)
        {
            var dbItem = await _kullaniciIpAdresleriRepository.GetByIdAsync(item.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            _kullaniciIpAdresleriRepository.Update(dbItem, item);
            var result = await _unitOfWork.SaveAsync();
            
            return new ResponseViewModel<KullaniciIpAdresleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> UpdateIpList(UpdateIpListDto updateIpListDto)
        {
            var items = await _kullaniciIpAdresleriRepository.GetByKullaniciIdAsync(updateIpListDto.KullaniciId);
            foreach (var item in items)
            {
                item.SilindiMi = true;
            }

            foreach (var item in updateIpListDto.IpAdresleri)
            {
                await _kullaniciIpAdresleriRepository.AddAsync(new KullaniciIpAdresleri
                {
                    KullaniciId = updateIpListDto.KullaniciId,
                    IpAdresi = item
                });
            }

            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullaniciIpAdresleriListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Delete(Guid id)
        {
            var item = await _kullaniciIpAdresleriRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullaniciIpAdresleriListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullaniciIpAdresleriListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }

    }
}
