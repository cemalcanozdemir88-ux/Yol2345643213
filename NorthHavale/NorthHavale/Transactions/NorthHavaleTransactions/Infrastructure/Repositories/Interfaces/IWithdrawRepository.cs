using NorthHavaleTransactions.Infrastructure.Dtos;
using NorthHavaleTransactions.Infrastructure.Entities;
using NorthHavaleTransactions.Models;

namespace NorthHavaleTransactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        Task<WithdrawTransaction> GetById(Guid Id);
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
        void CreateWithdraw(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto, Guid? tedarikciId, string bankName);
    }
}
