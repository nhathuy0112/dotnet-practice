using System.Collections;
using Application.Interfaces;
using Domain.Common;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool isDisposed;
    private readonly AppDbContext _context;
    private readonly Hashtable _repositories;
    
    public UnitOfWork(AppDbContext dbContext)
    {
        _context = dbContext;
        _repositories = new Hashtable();
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repoType = typeof(RepositoryBase<>);
            var repoInstance = Activator.CreateInstance(repoType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repoInstance);
        }

        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing)
        {
            _context.Dispose();
        }

        isDisposed = true;
    }
    
    ~UnitOfWork()
    {
        Dispose(false);
    }
}