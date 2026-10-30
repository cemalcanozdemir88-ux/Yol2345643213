using TedarikciHizmet.Core.Models;

namespace TedarikciHizmet.Core.Interfaces
{
    public interface IActionRequestLogRepository
    {
        Task AddAsync(ActionRequestLog entity);
    }
}
