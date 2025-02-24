using Contracts.Domain;
using EduTrailblaze.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Linq.Expressions;

namespace EduTrailblaze.Repositories
{
    public class Repository<T, TKey> : RepositoryQueryBase<T,TKey>, IRepository<T, TKey> where T : EntityBase<TKey>
    {
        private readonly EduTrailblazeDbContext _context;
        private readonly DbSet<T> _dbSet;
        

        public Repository(EduTrailblazeDbContext context) : base(context)
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

        
    }
}

