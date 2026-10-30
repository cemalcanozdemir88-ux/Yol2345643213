using TikkoPayYonetici.Core.DTOs.KullaniciBlackList;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IKullaniciBlackListsRepository : IGenericRepository<KullaniciBlackLists>
    {
        Task<IEnumerable<KullaniciBlackListDto>> GetAllDtoAsync(Guid? websiteId, long? userId, int pageNumber = 1, int recordsPerPage = 10);
        Task<int> GetCountAsync(Guid? websiteId, long? userId);
    }
}
