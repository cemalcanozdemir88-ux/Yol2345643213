using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IDepositRepository
    {
        Task<DepositTransaction> GetById(Guid Id);
        Task<DepositTransaction> GetWaitingDepositTransaction(Guid webSiteId, string userId);
        Guid Insert(DepositCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto);
    }
}
