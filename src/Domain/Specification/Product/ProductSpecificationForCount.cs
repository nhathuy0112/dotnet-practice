namespace Domain.Specification.Product;

public class ProductSpecificationForCount : SpecificationBase<Entities.Product>
{
    public ProductSpecificationForCount(ProductRequestParams requestParams)
    {
        int? priceMin = requestParams.PriceMin;
        string? name = requestParams.Name;
        int? categoryId = requestParams.CategoryId;
        int? priceMax = requestParams.PriceMax;

        Criteria = p => (string.IsNullOrEmpty(name) || p.Name.ToLower().Contains(name.ToLower()))
                        && (!categoryId.HasValue || p.CategoryId == categoryId)
                        && (!priceMin.HasValue || p.Price >= priceMin)
                        && (!priceMax.HasValue || p.Price <= priceMax);
    }
}