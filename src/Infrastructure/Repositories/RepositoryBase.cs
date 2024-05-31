using Application.Common.Interfaces;
using Domain.Common;
using Domain.QueryParams;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    private readonly AppDbContext _context;

    protected RepositoryBase(AppDbContext context)
    {
        _context = context;
    }

    public virtual async Task<List<T>> GetAsync(QueryParams queryParams)
    {
        var dbSet = _context.Set<T>().AsQueryable();
        dbSet = AddFilter(dbSet, queryParams);
        dbSet = AddOrderBy(dbSet, queryParams);
        dbSet = AddPaging(dbSet, queryParams);
        return await dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<int> CountAllAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public async Task<int> CountAsync(QueryParams queryParams)
    {
        var dbSet = _context.Set<T>().AsQueryable();
        dbSet = AddFilter(dbSet, queryParams);
        return await dbSet.CountAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    protected virtual IQueryable<T> AddFilter(IQueryable<T> dbSet, QueryParams queryParams)
    {
        return dbSet;
    }
    
    protected virtual IQueryable<T> AddOrderBy(IQueryable<T> dbSet, QueryParams queryParams)
    {
        if (string.IsNullOrEmpty(queryParams.Sort))
        {
            return dbSet;
        }
        
        var dbSetOrderBy = queryParams.Sort switch
        {
            "id-desc" => dbSet.OrderByDescending(x => x.Id),
            _ => dbSet.OrderBy(x => x.Id)
        };
        dbSet = dbSetOrderBy;

        return dbSet;
    }

    protected virtual IQueryable<T> AddPaging(IQueryable<T> dbSet, QueryParams queryParams)
    {
        dbSet = dbSet
            .Skip(queryParams.PageSize * (queryParams.PageIndex - 1))
            .Take(queryParams.PageSize);
        return dbSet;
    }
}  