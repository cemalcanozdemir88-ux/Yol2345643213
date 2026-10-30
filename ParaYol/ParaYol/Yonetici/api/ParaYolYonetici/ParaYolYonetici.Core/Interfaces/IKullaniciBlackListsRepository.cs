using ParaYolYonetici.Core.DTOs.KullaniciBlackList;
using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IKullaniciBlackListsRepository : IGenericRepository<KullaniciBlackLists>
    {
        Task<IEnumerable<KullaniciBlackListDto>> GetAllDtoAsync(Guid? websiteId, long? userId, int pageNumber = 1, int recordsPerPage = 10);
        Task<int> GetCountAsync(Guid? websiteId, long? userId);
    }
}
