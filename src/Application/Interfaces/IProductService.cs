using Application.Dto.Product;
using Application.Helpers;
using Domain.Specification.Product;

namespace Application.Interfaces;

public interface IProductService
{
    Task<PaginatedResponse<ProductResponse>> GetProductsAsync(ProductRequestParams requestParams);
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<ProductResponse> AddProductAsync(ProductRequest product, string createdBy);
    Task<ProductResponse> UpdateProductAsync(int id, ProductRequest product);
    Task<bool> DeleteProductAsync(int id);
}