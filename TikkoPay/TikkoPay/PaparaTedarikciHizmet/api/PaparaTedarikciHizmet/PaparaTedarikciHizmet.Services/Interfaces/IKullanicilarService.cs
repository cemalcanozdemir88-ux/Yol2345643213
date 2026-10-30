using PaparaTedarikciHizmet.Core.DTOs;
using PaparaTedarikciHizmet.Core.DTOs.Kullanicilar;
using PaparaTedarikciHizmet.Core.Enums;

namespace PaparaTedarikciHizmet.Services.Interfaces
{
    public interface IKullanicilarService
    {
        Task<ResponseViewModel<KullanicilarListDto>> GetById(Guid id);
        Task<ResponseViewModel<KullanicilarListDto>> GetByEmailPassword(LoginRequest loginRequest);
        Task<ResponseViewModel<List<KullanicilarListDto>>> GetAll(KullaniciTip? kullaniciTipi);
        Task<ResponseViewModel<KullanicilarListDto>> Create(KullanicilarCreateDto item);
        Task<ResponseViewModel<KullanicilarListDto>> ChangePassword(Guid kullaniciId, ChangePasswordDto changePasswordDto);
        Task<ResponseViewModel<KullanicilarListDto>> SetOtpEnabled(Guid kullaniciId);
        Task<ResponseViewModel<KullanicilarListDto>> Update(KullanicilarUpdateDto item);
        Task<ResponseViewModel<KullanicilarListDto>> Delete(Guid id);
    }
}
