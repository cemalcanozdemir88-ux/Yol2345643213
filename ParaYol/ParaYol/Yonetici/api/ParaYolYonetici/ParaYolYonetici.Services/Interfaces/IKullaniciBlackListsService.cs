using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.KullaniciBlackList;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IKullaniciBlackListsService
    {
        Task<ResponseViewModel<KullaniciBlackListDto>> Create(KullaniciBlackListsCreateDto item);
        Task<ResponseViewModel<KullaniciBlackListDto>> Delete(Guid id);
        Task<ResponseViewModelPagination<List<KullaniciBlackListDto>>> GetAll(Guid? websiteId, long? userId, int pageNumber, int recordsPerPage);
    }
}
