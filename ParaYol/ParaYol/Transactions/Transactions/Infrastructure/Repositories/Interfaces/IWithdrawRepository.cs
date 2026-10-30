using Transactions.Infrastructure.Dtos;
using Transactions.Infrastructure.Entities;
using Transactions.Models;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        Task<WithdrawTransaction> GetById(Guid Id);
        Task<WithdrawTransaction> GetByTransactionIdAsync(string transactionId);
        void CreateWithdraw(WithdrawCreateRequest request, WebSitePaymentMethodsDto webSitePaymentMethodDto, Guid? tedarikciId, string bankName);
    }
}
