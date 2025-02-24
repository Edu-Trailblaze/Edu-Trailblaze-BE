using Contracts.Common.Interfaces;
using Contracts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class RepositoryBaseAsync<T, K, TContext> : RepositoryQueryBase<T,K,TContext> ,  IRepositoryBaseAsync<T, K, TContext> where T : EntityBase<K> where TContext : DbContext
    {
        private readonly TContext _dbcontext;
        private readonly IUnitOfWork<TContext> _unitOfWork;
        public RepositoryBaseAsync(TContext dbcontext, IUnitOfWork<TContext> unitOfWork) : base(dbcontext)
        {
            _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public Task<IDbContextTransaction> BeginTransactionAsync()=> _dbcontext.Database.BeginTransactionAsync();

        public async Task<K> CreateAsync(T entity)
        {
            await _dbcontext.Set<T>().AddAsync(entity);
            return entity.Id;
        }

        public async Task<IList<K>> CreateListAsync(IEnumerable<T> entities)
        {
            await _dbcontext.Set<T>().AddRangeAsync(entities);
            return entities.Select(x => x.Id).ToList();
        }

        public Task DeleteAsync(T entity)
        {
            _dbcontext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task DeleteListAsync(IEnumerable<T> entities)
        {
            _dbcontext.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task EndTransactionoAsync()
        {
            await SaveChangesAsync();
            await _dbcontext.Database.CommitTransactionAsync();
        }

       
        public Task RollBackTransactionAsync() => _dbcontext.Database.RollbackTransactionAsync();

        public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();  

        public Task UpdateAsync(T entity)
        {
            if (_dbcontext.Entry(entity).State == EntityState.Unchanged) return Task.CompletedTask;
            T exist = _dbcontext.Set<T>().Find(entity.Id);
            _dbcontext.Entry(exist).CurrentValues.SetValues(entity);
            return Task.CompletedTask;
        }

        public Task UpdateListAsync(IEnumerable<T> entities) => _dbcontext.Set<T>().AddRangeAsync(entities);
    }
}
