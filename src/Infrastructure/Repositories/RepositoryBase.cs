using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RepositoryBase<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    
    public RepositoryBase(AppDbContext context)
    {
        _context = context;
    }

    protected async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    }

    protected async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    protected void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    protected void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    protected void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}  