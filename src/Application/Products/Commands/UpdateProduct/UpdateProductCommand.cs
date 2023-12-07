using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<UpdateProductResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
}