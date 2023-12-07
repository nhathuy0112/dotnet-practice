using Application.Common.Helpers;
using Domain.QueryParams.Product;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<PaginatedResponse<GetProductsResponse>>
{
    public GetProductsQuery(ProductQueryParams queryParams)
    {
        QueryParams = queryParams;
    }

    public ProductQueryParams QueryParams { get; }
}