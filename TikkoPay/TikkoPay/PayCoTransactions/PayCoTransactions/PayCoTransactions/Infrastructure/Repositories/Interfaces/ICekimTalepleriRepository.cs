using PayCoTransactions.Infrastructure.Entities;

namespace PayCoTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ICekimTalepleriRepository
    {
        Guid Insert(CekimTalepleri cekimTalepleri);
        void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult);
    }
}
