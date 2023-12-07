using Application.Common.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.CategoryRepository;
        var existedCategory = await categoryRepo.GetCategoryByIdAsync(command.Id);

         if (existedCategory == null)
         {
             throw new CategoryException("Cannot find category");
         }

         existedCategory.Name = command.Name;
         await categoryRepo.UpdateCategoryAsync(existedCategory);
         await _unitOfWork.CompleteAsync();
         return _mapper.Map<UpdateCategoryResponse>(existedCategory);
    }
}