using PayFixTransactions.Infrastructure.Entities;

namespace PayFixTransactions.Infrastructure.Repositories.Interfaces
{
    public interface ICekimTalepleriRepository
    {
        Guid Insert(CekimTalepleri cekimTalepleri);
        void UpdateWebSiteCallBackResult(Guid id, string webSiteCallBackResult);
    }
}
