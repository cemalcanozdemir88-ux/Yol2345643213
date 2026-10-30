using Transactions.Infrastructure.Entities;
using Transactions.Models;

namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IWithdrawRepository
    {
        void CreateWithdraw(WithdrawCreateRequest request, WebSiteleri webSiteItem);
    }
}
