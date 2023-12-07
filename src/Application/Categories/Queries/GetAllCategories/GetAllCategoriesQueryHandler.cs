using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponse>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();
        return _mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryResponse>>(categories);
    }
}