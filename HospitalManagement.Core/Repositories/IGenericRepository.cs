using System.Linq;
using System.Linq.Expressions;

public interface IGenericRepository<T> where T : class
{
    // Existing
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(object id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    IQueryable<T> Query(bool asNoTracking = true);
    IQueryable<T> QueryWithIncludes(bool asNoTracking = true, params Expression<Func<T, object>>[] includes);
}
