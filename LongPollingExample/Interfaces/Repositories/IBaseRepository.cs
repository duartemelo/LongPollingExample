using LongPollingExample.Entities;

namespace LongPollingExample.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> AddAsync(T entity);
        void Delete(T entity);
        IQueryable<T> GetEntity();
    }
}
