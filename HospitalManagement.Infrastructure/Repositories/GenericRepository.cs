using HospitalManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly HospitalDbContext _context;
    private readonly DbSet<T> _set;

    public GenericRepository(HospitalDbContext context)
    {
        _context = context;
        _set = _context.Set<T>();
    }

    // NEW
    public IQueryable<T> Query(bool asNoTracking = true)
        => asNoTracking ? _set.AsNoTracking() : _set.AsQueryable();

    // NEW
    public IQueryable<T> QueryWithIncludes(bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _set.AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);
        return asNoTracking ? query.AsNoTracking() : query;
    }

    // Existing
    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _set.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _set.AsQueryable();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id) =>
        await _set.FindAsync(id);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _set.AsNoTracking().Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) =>
        await _set.AddAsync(entity);

    public void Update(T entity) =>
        _set.Update(entity);

    public void Delete(T entity) =>
        _set.Remove(entity);
}
