using BankingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _AppDbcontext;

        public GenericRepository(AppDbContext appDbcontext)
        {
            _AppDbcontext = appDbcontext;
        }

        public async Task<T?> GetByIdAsync(int id, bool trackChanges)
        {
            var Entity = await _AppDbcontext.Set<T>().FindAsync(id);

            if (Entity != null && trackChanges == false)
            {
                _AppDbcontext.Entry(Entity).State = EntityState.Detached;
            }
            return Entity;
        }

        public async Task<T?> AddAsync(T entity)
        {
            await _AppDbcontext.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _AppDbcontext.Set<T>().AddRangeAsync(entities);
        }

        public Task Update(T entity)
        {
            _AppDbcontext.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _AppDbcontext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
        public async Task<(IEnumerable<T> Items, int TotalCount)> ApplyPaginationAsync(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var TotalCount = await query.CountAsync();
            int totalPages = TotalCount / pageSize;
            var pagedResult = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();
            return (pagedResult, TotalCount);
        }
    }
}