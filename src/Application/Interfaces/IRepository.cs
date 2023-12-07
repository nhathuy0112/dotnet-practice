using Domain.Common;
using Domain.Specification;

namespace Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetAsync(ISpecification<T> specification);
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetListAsync(ISpecification<T> specification);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<int> CountAsync(ISpecification<T> specification);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}