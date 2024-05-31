using Domain.Common;
using Domain.QueryParams;

namespace Application.Common.Interfaces;

public interface IRepositoryBase<T> where T : BaseEntity
{
    Task<List<T>> GetAsync(QueryParams queryParams); 
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<int> CountAllAsync();
    Task<int> CountAsync(QueryParams queryParams);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}