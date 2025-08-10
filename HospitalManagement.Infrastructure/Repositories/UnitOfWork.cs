using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalManagement.Core.Repositories;

namespace HospitalManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(HospitalDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.TryGetValue(type, out var repo))
            {
                repo = new GenericRepository<T>(_context);
                _repositories[type] = repo!;
            }
            return (IGenericRepository<T>)repo!;
        }

        public Task<int> SaveAsync() => _context.SaveChangesAsync();

        #region IDisposable
        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
