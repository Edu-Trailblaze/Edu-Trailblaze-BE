using EduTrailblaze.API.Domain;
using EduTrailblaze.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EduTrailblaze.Repositories
{
    public class RepositoryQueryBase<T, K> : IRepositoryQueryBase<T, K> where T : EntityBase<K>
    {
        private readonly EduTrailblazeDbContext _context;
        private readonly DbSet<T> _dbSet;


        public RepositoryQueryBase(EduTrailblazeDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public IQueryable<T> FindAll(bool trackChanges = false)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {

            IQueryable<T> query = _dbSet;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return query.Where(expression);
        }

        public Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            var entity = query.FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (entity == null)
            {
                throw new Exception("Entity not found");
            }

            return entity;
        }

        public Task<T?> GetByIdAsync(K id)
        {
            throw new NotImplementedException();
        }

       
    }
}
