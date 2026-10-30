using ParaYolYonetici.Core.Models;

namespace ParaYolYonetici.Core.Interfaces
{
    public interface IActionRequestLogRepository
    {
        Task AddAsync(ActionRequestLog entity);
    }
}
