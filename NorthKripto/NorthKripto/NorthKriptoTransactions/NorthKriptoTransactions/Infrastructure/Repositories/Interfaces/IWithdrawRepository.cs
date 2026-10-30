using NorthKriptoTransactions.Infrastructure.Dtos;
using NorthKriptoTransactions.Infrastructure.Entities;
using NorthKriptoTransactions.Models;

namespace NorthKriptoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        Task<WithdrawTransaction> GetById(Guid Id);
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
        void CreateWithdraw(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto);
    }
}
