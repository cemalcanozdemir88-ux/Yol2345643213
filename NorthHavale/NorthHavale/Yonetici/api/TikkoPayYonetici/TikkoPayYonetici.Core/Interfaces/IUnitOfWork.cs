namespace TikkoPayYonetici.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
