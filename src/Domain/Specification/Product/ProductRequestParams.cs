namespace Domain.Specification.Product;

public class ProductRequestParams : RequestParams
{
    public int? PriceMin { get; set; } 
    public int? PriceMax { get; set; }
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
}