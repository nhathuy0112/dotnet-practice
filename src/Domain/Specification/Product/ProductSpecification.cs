namespace Domain.Specification.Product;

public class ProductSpecification : SpecificationBase<Entities.Product>
{
    public ProductSpecification(ProductRequestParams requestParams)
    {
        int? priceMin = requestParams.PriceMin;
        string? name = requestParams.Name;
        int? categoryId = requestParams.CategoryId;
        int? priceMax = requestParams.PriceMax;

        Criteria = p => (string.IsNullOrEmpty(name) || p.Name.ToLower().Contains(name.ToLower()))
                        && (!categoryId.HasValue || p.CategoryId == categoryId)
                        && (!priceMin.HasValue || p.Price >= priceMin)
                        && (!priceMax.HasValue || p.Price <= priceMax);
        ApplyPaging(requestParams.PageSize * (requestParams.PageIndex -1), requestParams.PageSize);

        if (!string.IsNullOrEmpty(requestParams.Sort))
        {
            switch (requestParams.Sort)
            {
                case "name":
                    AddOrderBy(p => p.Name);
                    break;
                case "name-desc":
                    AddOrderByDescending(p => p.Name);
                    break;
                case "price":
                    AddOrderBy(p => p.Price);
                    break;
                case "price-desc":
                    AddOrderByDescending(p => p.Price);
                    break;
                case "date":
                    AddOrderBy(p => p.CreatedDate);
                    break;
                case "date-desc":
                    AddOrderByDescending(p => p.CreatedDate);
                    break;
                default:
                    AddOrderBy(p => p.Id);
                    break;
            }
        }
    }
}