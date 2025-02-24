using Contracts.Domain;
using System.Linq.Expressions;

namespace EduTrailblaze.Repositories.Interfaces
{
    public interface IRepository<T, TKey> : IRepositoryQueryBase<T, TKey> where T : EntityBase<TKey>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IQueryable<T>> GetDbSet();
        // Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
    public interface IRepositoryQueryBase<T, K>
       where T : EntityBase<K>
    {
        IQueryable<T> FindAll(bool trackChanges = false);
        IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> FindByCondition(System.Linq.Expressions.Expression<Func<T, bool>> expression, bool trackChanges = false);
        IQueryable<T> FindByCondition(System.Linq.Expressions.Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetByIdAsync(K id);
        Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperty);
    }
}
