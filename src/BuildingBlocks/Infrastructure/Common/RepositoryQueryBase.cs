using Contracts.Common.Interfaces;
using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class RepositoryQueryBase<T, K, TContext> : IRepositoryQueryBase<T, K, TContext> where T : EntityBase<K> where TContext : DbContext
    {
        private readonly TContext _dbcontext;
        public RepositoryQueryBase(TContext dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
        }
        public IQueryable<T> FindAll(bool trackChanges = false) =>
            !trackChanges ? _dbcontext.Set<T>().AsNoTracking() : _dbcontext.Set<T>();
        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var item = FindAll(trackChanges);
            item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
            return item;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
            !trackChanges ? _dbcontext.Set<T>().Where(expression).AsNoTracking() : _dbcontext.Set<T>().Where(expression);

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var item = FindByCondition(expression, trackChanges);
            item = includeProperties.Aggregate(item, (current, includeProperty) => current.Include(includeProperty));
            return item;
        }

        public async Task<T?> GetByIdAsync(K id) =>
            await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperty) =>
            await FindByCondition(x => x.Id.Equals(id), false, includeProperty).FirstOrDefaultAsync();

    }
}
