using Application.Common.Interfaces;
using Domain.Entities;
using Domain.QueryParams.Product;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductQueryParams queryParams)
    {
        var products = _context.Products.AsQueryable();
        products = AddFilter(products, queryParams);
        products = AddOrderBy(products, queryParams);
        products = AddPaging(products, queryParams);
        return await Task.FromResult(products.ToList());
    }

    public async Task<int> CountProductsAsync(ProductQueryParams queryParams)
    {
        var products = _context.Products.AsQueryable();
        products = AddFilter(products, queryParams);
        return await Task.FromResult(products.Count());
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public Task AddProductAsync(Product product)
    {
        Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateProductAsync(Product product)
    {
        Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteProductAsync(Product product)
    {
        Delete(product);
        return Task.CompletedTask;
    }

    private IQueryable<Product> AddFilter(IQueryable<Product> products, ProductQueryParams queryParams)
    {
        var filter = (Product p) => (string.IsNullOrEmpty(queryParams.Name) || p.Name.ToLower().Contains(queryParams.Name.ToLower()))
                            && (!queryParams.CategoryId.HasValue || p.CategoryId == queryParams.CategoryId)
                            && (!queryParams.PriceMin.HasValue || p.Price >= queryParams.PriceMin)
                            && (!queryParams.PriceMax.HasValue || p.Price <= queryParams.PriceMax);
        products = products.Where(filter).AsQueryable();
        return products;
    }

    private IQueryable<Product> AddOrderBy(IQueryable<Product> products, ProductQueryParams queryParams)
    {
        if (!string.IsNullOrEmpty(queryParams.Sort))
        {
            var productOrderBy = queryParams.Sort switch
            {
                "name" => products.OrderBy(p => p.Name),
                "name-desc" => products.OrderByDescending(p => p.Name),
                "price" => products.OrderBy(p => p.Price),
                "price-desc" => products.OrderByDescending(p => p.Price),
                "date" => products.OrderBy(p => p.CreatedDate),
                "date-desc" => products.OrderByDescending(p => p.CreatedDate),
                _ => products.OrderBy(p => p.Id)
            };
            products = productOrderBy;
            return products;
        }

        return products;
    }

    private IQueryable<Product> AddPaging(IQueryable<Product> products, ProductQueryParams queryParams)
    {
        products = products
            .Skip(queryParams.PageSize * (queryParams.PageIndex - 1))
            .Take(queryParams.PageSize);
        return products;
    }
    
}