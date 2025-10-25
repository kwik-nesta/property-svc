using KwikNesta.Property.Svc.Domain.Models;
using KwikNesta.Property.Svc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AsNoTracking()
                .Where(expression)
                .ToListAsync();
        }

        public IQueryable<T> GetAsQueryable(Expression<Func<T, bool>> expression)
        {
            return _dbSet.AsNoTracking()
                .Where(expression);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity, bool saveNow = true)
        {
            await _dbSet.AddAsync(entity);
            if (saveNow)
                await SaveAsync();
        }

        public async Task Update(T entity, bool saveNow = true)
        {
            _dbSet.Update(entity);
            if (saveNow)
                await SaveAsync();
        }

        public async Task Delete(T entity, bool saveNow = true)
        {
            _dbSet.Remove(entity);
            if (saveNow)
                await SaveAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.LongCountAsync(expression);
        }

        private async Task<bool> SaveAsync(CancellationToken cancellation = default)
        {
            return await _dbContext.SaveChangesAsync(cancellation) > 0;
        }
    }
}