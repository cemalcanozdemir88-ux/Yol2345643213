using AutoMapper;
using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.Kullanicilar;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Interfaces;
using TedarikciHizmet.Core.Models;
using TedarikciHizmet.Services.Interfaces;

namespace TedarikciHizmet.Services.Concretes
{
    public class KullanicilarService : IKullanicilarService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
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
            Kullanicilar model = _mapper.Map<Kullanicilar>(item);
            model.SetPassword(item.Sifre);
            model.IsOtpEnabled = false;
            model.IpLoggingEnabled = true;

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

            if(!string.IsNullOrEmpty(itemDto.Sifre))
            {
                dbItem.SetPassword(itemDto.Sifre);
            }

            _kullanicilarRepository.Update(dbItem, itemDto);
            _ = await _unitOfWork.SaveAsync();

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

            _kullanicilarRepository.Delete(item);
            await _unitOfWork.SaveAsync();

            return new ResponseViewModel<KullanicilarListDto>
            {
                Success = true,
                Message = "Kayıt silindi"
            };
        }
    }
}
