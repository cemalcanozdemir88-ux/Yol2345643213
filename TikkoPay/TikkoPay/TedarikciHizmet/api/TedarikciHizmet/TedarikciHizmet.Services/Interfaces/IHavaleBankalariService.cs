using TedarikciHizmet.Core.DTOs;
using TedarikciHizmet.Core.DTOs.HavaleBankalari;
using TedarikciHizmet.Core.Enums;

namespace TedarikciHizmet.Services.Interfaces
{
    public interface IHavaleBankalariService
    {
        Task<ResponseViewModel<HavaleBankalariListDto>> GetById(Guid id);
        Task<ResponseViewModel<List<HavaleBankalariListDto>>> GetAll(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter);
        Task<ResponseViewModelPagination<List<HavaleBankalariListDto>>> GetAll(Guid? bankaId, string searchText, HavaleBankalariDurumFilter havaleBankalariDurumFilter, int pageNumber, int recordsPerPage);
        Task<ResponseViewModel<HavaleBankalariListDto>> Create(HavaleBankalariCreateDto item);
        Task<ResponseViewModel<HavaleBankalariListDto>> Update(HavaleBankalariUpdateDto item);
        Task<ResponseViewModel<HavaleBankalariListDto>> Delete(Guid id);
    }
}
