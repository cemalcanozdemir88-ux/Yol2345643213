using TedarikciHizmet.Core.DTOs.YatirimTalepleri;
using TedarikciHizmet.Core.Enums;
using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IYatirimCekimTalepleriRepository : IGenericRepository<YatirimCekimTalepleri>
    {
        Task<YatirimTalepleriListDto> GetByIdDtoAsync(Guid id);
        Task<int> GetCountAsync(Guid? tedarikciId, IslemTip islemTipi, Durum durum);
        Task<int> GetCountAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<IEnumerable<YatirimTalepleriListDto>> GetAllAsync(Guid? tedarikciId, Guid? bankaId, string searchText, IslemTip islemTipi, Durum durum, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd, int pageNumber, int recordsPerPage);
        Task<ToplamYatirimTutariDto> GetToplamYatirimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
        Task<ToplamCekimTutariDto> GetToplamCekimTutari(Guid tedarikciId, Guid? bankaId, string searchText, DateTime? approveDateStart, DateTime? approveDateEnd, DateTime? createdDateStart, DateTime? createdDateEnd);
    }
}
