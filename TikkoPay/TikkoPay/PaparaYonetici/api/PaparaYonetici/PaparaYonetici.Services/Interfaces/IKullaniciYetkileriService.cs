using PaparaYonetici.Core.DTOs;
using PaparaYonetici.Core.DTOs.KullaniciYetkileri;

namespace PaparaYonetici.Services.Interfaces
{
    public interface IKullaniciYetkileriService
    {
        Task<ResponseViewModel<KullaniciYetkileriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KullaniciYetkileriListDto>>> GetByKullaniciId(Guid kullaniciId);
        Task<ResponseViewModel<KullaniciYetkileriListDto>> Create(KullaniciYetkileriCreateDto item);
        Task<ResponseViewModel<KullaniciYetkileriListDto>> Update(KullaniciYetkileriUpdateDto item);
        Task<ResponseViewModel<KullaniciYetkileriListDto>> Delete(KullaniciYetkileriDeleteDto item);
    }
}
