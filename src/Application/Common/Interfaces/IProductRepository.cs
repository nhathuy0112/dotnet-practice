using Domain.Entities;
using Domain.QueryParams.Product;

namespace Application.Common.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(ProductQueryParams queryParams);
    Task<int> CountProductsAsync(ProductQueryParams queryParams);
    Task<Product?> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
}