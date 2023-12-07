namespace Application.Products.Queries.GetProducts;

public class GetProductsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string CreatedDate { get; set; }
    public int CategoryId { get; set; }
    public string CreatedBy { get; set; }
}