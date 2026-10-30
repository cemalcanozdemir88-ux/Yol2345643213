namespace PaparaYonetici.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
