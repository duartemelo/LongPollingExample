using LongPollingExample.Data;
using LongPollingExample.Entities;
using LongPollingExample.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LongPollingExample.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetEntity()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
