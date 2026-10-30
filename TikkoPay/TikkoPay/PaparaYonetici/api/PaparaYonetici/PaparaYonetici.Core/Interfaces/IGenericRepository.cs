using System.Linq.Expressions;

namespace PaparaYonetici.Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity, object dto);
    }
}
