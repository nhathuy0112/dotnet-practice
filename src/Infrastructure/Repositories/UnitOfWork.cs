using System.Collections;
using Application.Common.Interfaces;
using Domain.Common;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool isDisposed;
    private readonly AppDbContext _context;
    private CategoryRepository _categoryRepository;
    private ProductRepository _productRepository;
    private TokenRepository _tokenRepository;
    
    public UnitOfWork(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            if (_categoryRepository == null)
            {
                _categoryRepository = new(_context);
            }

            return _categoryRepository;
        }
    }

    public IProductRepository ProductRepository
    {
        get
        {
            if (_productRepository == null)
            {
                _productRepository = new(_context);
            }

            return _productRepository;
        }
    }

    public ITokenRepository TokenRepository
    {
        get
        {
            if (_tokenRepository == null)
            {
                _tokenRepository = new(_context);
            }

            return _tokenRepository;
        }
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