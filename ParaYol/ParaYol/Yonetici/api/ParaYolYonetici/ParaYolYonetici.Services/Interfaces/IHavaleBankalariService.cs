using ParaYolYonetici.Core.DTOs;
using ParaYolYonetici.Core.DTOs.HavaleBankalari;
using ParaYolYonetici.Core.Enums;

namespace ParaYolYonetici.Services.Interfaces
{
    public interface IHavaleBankalariService
    {
        Task<ResponseViewModel<HavaleBankalariListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<HavaleBankalariListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<ResponseViewModelPagination<List<HavaleBankalariListDto>>> GetAll(Guid? tedarikciId, Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<HavaleBankalariListDto>> Create(HavaleBankalariCreateDto item);
        Task<ResponseViewModel<HavaleBankalariListDto>> Update(HavaleBankalariUpdateDto item);
        Task<ResponseViewModel<HavaleBankalariListDto>> Delete(Guid id);
        Task<ResponseViewModel<List<HavaleBankalariListDto>>> GetAllByWebSiteTedarikcileri(Guid webSiteId);
    }
}
