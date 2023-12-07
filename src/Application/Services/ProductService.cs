using Application.Dto.Product;
using Application.Helpers;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specification.Product;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ProductResponse>> GetProductsAsync(ProductRequestParams requestParams)
    {
        var productRepo = _unitOfWork.Repository<Product>();
        var productSpec = new ProductSpecification(requestParams);
        var productSpecForCount = new ProductSpecificationForCount(requestParams);
        var count = await productRepo.CountAsync(productSpecForCount);
        var products = await productRepo.GetListAsync(productSpec);
        var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(products);
        return new PaginatedResponse<ProductResponse>()
        {
            PageIndex = requestParams.PageIndex,
            PageSize = requestParams.PageSize,
            Count = count,
            Data = data
        };
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

        if (product == null)
        {
            throw new ProductException("Cannot find product");
        }
        
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse> AddProductAsync(ProductRequest product, string createdBy)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);

        if (category == null)
        {
            throw new ProductException("Cannot add product. Category is invalid");
        }
        
        var newProduct = new Product()
        {
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CreatedDate = DateTime.Now,
            CreatedBy = createdBy
        };
        _unitOfWork.Repository<Product>().Add(newProduct);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ProductResponse>(newProduct);
    }

    public async Task<ProductResponse> UpdateProductAsync(int id, ProductRequest product)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);

        if (category == null)
        {
            throw new ProductException("Cannot update product. Category is invalid");
        }
        
        var productRepo = _unitOfWork.Repository<Product>();
        var existedProduct = await productRepo.GetByIdAsync(id);
        
        if (existedProduct == null)
        {
            throw new ProductException("Cannot find product");
        }

        existedProduct.Name = product.Name;
        existedProduct.CategoryId = product.CategoryId;
        existedProduct.Price = product.Price;
        productRepo.Update(existedProduct);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ProductResponse>(existedProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var productRepo = _unitOfWork.Repository<Product>();
        var existedProduct = await productRepo.GetByIdAsync(id);
        
        if (existedProduct == null)
        {
            throw new ProductException("Cannot find product");
        }
        
        productRepo.Delete(existedProduct);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}