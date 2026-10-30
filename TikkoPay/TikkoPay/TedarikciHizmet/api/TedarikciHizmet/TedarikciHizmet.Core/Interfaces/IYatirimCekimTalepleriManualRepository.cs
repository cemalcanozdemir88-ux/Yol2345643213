using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.DTOs.YatirimTalepleriManual;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IYatirimCekimTalepleriManualRepository : IGenericRepository<YatirimCekimTalepleriManual>
    {
        Task<YatirimTalepleriManualListDto> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, IslemTip islemTipi, Durum durum);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriManualListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriManualListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);

    }
}
