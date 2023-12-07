using MediatR;

namespace Application.Products.Commands.AddProduct;

public class AddProductCommand : IRequest<AddProductResponse>
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
    public string CreatedBy { get; set; }
}