using Application.Common.Interfaces;
using Domain.Entities;
using Domain.QueryParams;
using Domain.QueryParams.Product;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<List<Product>> GetAsync(QueryParams queryParams)
    {
        var products = _context.Products.AsQueryable();
        products = AddFilter(products, queryParams);
        products = AddOrderBy(products, queryParams);
        products = AddPaging(products, queryParams);
        return await products.ToListAsync();
    }

    protected override IQueryable<Product> AddFilter(IQueryable<Product> products, QueryParams queryParams)
    {
        if (queryParams is not ProductQueryParams productQueryParams)
        {
            throw new Exception($"{nameof(ProductRepository)}->{nameof(AddFilter)} query params is not type of {nameof(ProductQueryParams)}");
        }
        
        var filter = 
            (Product p) => (string.IsNullOrEmpty(productQueryParams.Name) || p.Name.ToLower().Contains(productQueryParams.Name.ToLower())) && 
                           (!productQueryParams.CategoryId.HasValue || p.CategoryId == productQueryParams.CategoryId) && 
                           (!productQueryParams.PriceMin.HasValue || p.Price >= productQueryParams.PriceMin) && 
                           (!productQueryParams.PriceMax.HasValue || p.Price <= productQueryParams.PriceMax);
        
        products = products
            .AsEnumerable()
            .Where(filter)
            .AsQueryable();

        return products;
    }

    protected override IQueryable<Product> AddOrderBy(IQueryable<Product> products, QueryParams queryParams)
    {
        if (queryParams is not ProductQueryParams productQueryParams)
        {
            throw new Exception($"{nameof(ProductRepository)}->{nameof(AddFilter)} query params is not type of {nameof(ProductQueryParams)}");
        }
        
        if (string.IsNullOrEmpty(queryParams.Sort))
        {
            return products;
        }
        
        var productOrderBy = productQueryParams.Sort switch
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
}
