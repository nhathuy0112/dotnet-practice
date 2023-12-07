using Application.Common.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using MediatR;

namespace Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetProductByIdAsync(query.Id);

        if (product == null)
        {
            throw new ProductException("Cannot find product");
        }

        return _mapper.Map<GetProductByIdResponse>(product);
    }
}