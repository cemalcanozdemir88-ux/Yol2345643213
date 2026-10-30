using ParaYolYonetici.Core.DTOs.Tedarikciler;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface ITedarikciRepository : IGenericRepository<Tedarikciler>
    {
        Task<List<TedarikcilerSelectListWithBakiyeDto>> GetSelectListWithBakiyeAsync();
        Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId, int? tedarikciDurum);
        Task<int> GetCountAsync(bool? bankaHavale, bool? otoPuan, bool? durum);
        Task<IEnumerable<TedarikcilerListDto>> GetAllDtoAsync(bool? bankaHavale, bool? otoPuan, bool? durum, int pageNumber = 1, int recordsPerPage = 10);
        Task<List<TedarikcilerSelectList>> GetSelectListAsync();
    }
}
