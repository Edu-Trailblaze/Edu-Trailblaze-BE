using EduTrailblaze.API.Domain;
using EduTrailblaze.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Linq.Expressions;

namespace EduTrailblaze.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : EntityBase<TKey>
    {
        private readonly EduTrailblazeDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(EduTrailblazeDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public Task<IQueryable<T>> GetDbSet()
        {
            try
            {
                var dbSet = Task.FromResult(_dbSet.AsQueryable());
                if (dbSet == null)
                {
                    throw new InvalidOperationException("DbSet returned null.");
                }
                return dbSet;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                IQueryable<T> query = _dbSet;
                var isDeleteProperty = typeof(T).GetProperty("IsDeleted");
                if (isDeleteProperty != null) query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
                var entity = await query.AsQueryable().ToListAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            try
            {
                IQueryable<T> query = _dbSet;
                var isDeleteProperty = typeof(T).GetProperty("IsDeleted");
                if (isDeleteProperty != null) query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
                var entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));

                if (entity == null)
                {
                    return null;
                }
                return entity;
            }
            catch (SqlNullValueException ex)
            {
                throw new Exception("A field in the database contains a NULL value", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entity: {ex.Message}", ex);
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {

                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't add entity: {ex.InnerException?.Message ?? ex.Message}");

            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't update entity: {ex.Message}");
            }
        }

        public async Task DeleteAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't delete entity: {ex.Message}");
            }
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

        public Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includeProperties)
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
    }
}

