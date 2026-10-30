namespace ParaYolYonetici.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveAsync(bool setBaseModelValues = true);
    }
}
