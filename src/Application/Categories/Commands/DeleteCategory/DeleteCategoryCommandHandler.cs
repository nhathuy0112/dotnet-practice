using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.CategoryRepository;
        var existedCategory = await categoryRepo.GetByIdAsync(command.Id);

         if (existedCategory == null)
         {
             throw new CategoryException("Cannot find category");
         }

         categoryRepo.Delete(existedCategory);
         await _unitOfWork.CompleteAsync();
         return true;
    }
}