namespace Domain.QueryParams.Product;

public class ProductQueryParams : QueryParams
{
    public int? PriceMin { get; set; } 
    public int? PriceMax { get; set; }
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
}