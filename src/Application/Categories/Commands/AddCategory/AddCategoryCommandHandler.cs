using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Categories.Commands.AddCategory;

public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, AddCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AddCategoryResponse> Handle(AddCategoryCommand command, CancellationToken cancellationToken)
    {
        var newCategory = new Category()
        {
            Name = command.Name
        };
        await _unitOfWork.CategoryRepository.AddAsync(newCategory);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<AddCategoryResponse>(newCategory);
    }
}