using System.Linq.Expressions;

namespace apiprac
{
    public interface IRepository<T>
    {
        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task SaveAsync();

        Task<List<T>> FindByCriteriaAsync(Expression<Func<T, bool>>? filter = null);

        Task<T> FindByIdAsync(Expression<Func<T, bool>>? filter = null);
    }
}
