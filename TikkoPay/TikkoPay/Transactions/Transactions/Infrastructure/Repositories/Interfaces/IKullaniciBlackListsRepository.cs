
namespace Transactions.Infrastructure.Repositories.Interfaces
{
    public interface IKullaniciBlackListsRepository
    {
        Task<bool> IsUserInBlacklistAsync(Guid websiteId, string userId);
    }
}
