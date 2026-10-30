using TikkoPayYonetici.Core.Models;

namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IActionRequestLogRepository
    {
        Task AddAsync(ActionRequestLog entity);
    }
}
