using AutoMapper;
using PayCoYonetici.Core.DTOs;
using PayCoYonetici.Core.DTOs.Kullanicilar;
using PayCoYonetici.Core.Enums;
using PayCoYonetici.Core.Interfaces;
using PayCoYonetici.Core.Models;
using PayCoYonetici.Services.Interfaces;

namespace PayCoYonetici.Services.Concretes
{
    public class KullanicilarService : IKullanicilarService
    {
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private readonly IKullanicilarRepository _kullanicilarRepository;

        public KullanicilarService(IMapper mapper, IUnitOfWork unitOfWork, IKullanicilarRepository kullanicilarRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _kullanicilarRepository = kullanicilarRepository;
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> GetById(Guid id)
        {
            var item = await _kullanicilarRepository.GetByIdDtoAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KullanicilarListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> GetByEmailPassword(LoginRequest loginRequest)
        {
            var item = await _kullanicilarRepository.GetByEmailPassword(loginRequest);
            if (item == null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı"
                };
            }
            else
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = true,
                    Result = _mapper.Map<KullanicilarListDto>(item)
                };
            }
        }

        public async Task<ResponseViewModel<List<KullanicilarListDto>>> GetAll(KullaniciTip? kullaniciTipi)
        {
            var items = await _kullanicilarRepository.GetAllDtoAsync(kullaniciTipi);
            return new ResponseViewModel<List<KullanicilarListDto>>
            {
                Success = true,
                Result = _mapper.Map<List<KullanicilarListDto>>(items)
            };
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> Create(KullanicilarCreateDto item)
        {
            var isUniqueEmail = await _kullanicilarRepository.IsUniqueEmail(item.Eposta.Trim());
            if (!isUniqueEmail)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "E-Posta adresi kullanılmaktadır."
                };
            }

            Kullanicilar model = _mapper.Map<Kullanicilar>(item);
            model.SetPassword(item.Sifre);

            await _kullanicilarRepository.AddAsync(model);
            var result = await _unitOfWork.SaveAsync();

            if (result > 0)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = true
                };
            }
            else
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Hata oluştu, bir süre sonra tekrar deneyiniz."
                };
            }
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> ChangePassword(Guid kullaniciId, ChangePasswordDto changePasswordDto)
        {
            if (!changePasswordDto.NewPassword.Equals(changePasswordDto.NewPasswordConfirm))
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Şifreler uyuşmuyor!"
                };
            }

            var kullanici = await _kullanicilarRepository.GetByIdAsync(kullaniciId);
            if (kullanici is null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı!"
                };
            }

            var verifyPasswordResult = kullanici.VerifyPassword(changePasswordDto.OldPassword);
            if (!verifyPasswordResult)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Eski şifre hatalı!"
                };
            }

            kullanici.SetPassword(changePasswordDto.NewPassword);
            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullanicilarListDto>
            {
                Success = true
            };

        }

        public async Task<ResponseViewModel<KullanicilarListDto>> SetOtpEnabled(Guid kullaniciId)
        {
            var dbItem = await _kullanicilarRepository.GetByIdAsync(kullaniciId);
            if (dbItem == null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            dbItem.IsOtpEnabled = true;
            _ = await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullanicilarListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> Update(KullanicilarUpdateDto itemDto)
        {
            var dbItem = await _kullanicilarRepository.GetByIdAsync(itemDto.Id);
            if (dbItem == null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Kayıt bulunamadı."
                };
            }

            if (!dbItem.Eposta.Equals(itemDto.Eposta))
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "E-Posta adresi değiştirilemez"
                };
            }

            if (!string.IsNullOrEmpty(itemDto.Sifre))
            {
                dbItem.SetPassword(itemDto.Sifre);
            }

            _kullanicilarRepository.Update(dbItem, itemDto);
            var result = await _unitOfWork.SaveAsync();
            return new ResponseViewModel<KullanicilarListDto>
            {
                Success = true
            };
        }

        public async Task<ResponseViewModel<KullanicilarListDto>> Delete(Guid id)
        {
            var item = await _kullanicilarRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ResponseViewModel<KullanicilarListDto>
                {
                    Success = false,
                    Message = "Silinecek kayıt bulunamadı"
                };
            }

            item.SilindiMi = true;
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullanicilarListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
