using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<bool>
{
    public DeleteProductCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}