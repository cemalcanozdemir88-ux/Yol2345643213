using TikkoPayYonetici.Core.DTOs;
using TikkoPayYonetici.Core.DTOs.KullaniciIpAdresleri;

namespace TikkoPayYonetici.Services.Interfaces
{
    public interface IKullaniciIpAdresleriService
    {
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<KullaniciIpAdresleriListDto>>> GetAll(Guid? tedarikciId, string searchText);
        Task<ResponseViewModel<KullaniciIpAdresleriWithAvailableCount>> GetByKullaniciId(Guid kullaniciId);
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Create(KullaniciIpAdresleriCreateDto item);
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Update(KullaniciIpAdresleriUpdateDto item);
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> Delete(Guid id);
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> UpdateIpList(UpdateIpListDto updateIpListDto);
        Task<ResponseViewModel<KullaniciIpAdresleriListDto>> CreateIfAvaliable(KullaniciIpAdresleriCreateDto item);
    }
}
