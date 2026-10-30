namespace PayFixYonetici.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
