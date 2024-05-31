using Application.Common.Helpers;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.QueryParams.Product;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResponse<GetProductsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<GetProductsResponse>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.ProductRepository.GetAsync(query.QueryParams);
        var count = await _unitOfWork.ProductRepository.CountAsync(query.QueryParams);
        var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<GetProductsResponse>>(products);
        return new PaginatedResponse<GetProductsResponse>()
        {
            PageSize = query.QueryParams.PageSize,
            PageIndex = query.QueryParams.PageIndex,
            Count = count,
            Data = data
        };
    }
}