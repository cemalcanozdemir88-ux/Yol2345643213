using TikkoPayYonetici.Core.DTOs.Tedarikciler;
using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface ITedarikciRepository : IGenericRepository<Tedarikciler>
    {
        Task<List<TedarikcilerSelectListWithBakiyeDto>> GetSelectListWithBakiyeAsync();
        Task<List<TedarikciBakiyeListDto>> GetBakiyeList(Guid? tedarikciId);
    }
}
