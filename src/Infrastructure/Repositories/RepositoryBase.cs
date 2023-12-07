using Application.Interfaces;
using Domain.Common;
using Domain.Specification;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    
    public RepositoryBase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetAsync(ISpecification<T> specification)
    {
        return await Task.FromResult(ApplySpecification(specification).FirstOrDefault());
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetListAsync(ISpecification<T> specification)
    {
        return await Task.FromResult(ApplySpecification(specification).ToList());
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> specification)
    {
        return await Task.FromResult(ApplySpecification(specification).Count());
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
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
    
    private IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
    }
}  