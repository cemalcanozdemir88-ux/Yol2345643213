namespace TedarikciHizmet.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync();
    }
}
