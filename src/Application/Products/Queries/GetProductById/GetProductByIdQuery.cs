using MediatR;

namespace Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<GetProductByIdResponse>
{
    public GetProductByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}