using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var productRepo = _unitOfWork.ProductRepository;
         var existedProduct = await productRepo.GetByIdAsync(command.Id);
         
         if (existedProduct == null)
         {
             throw new ProductException("Cannot find product");
         }
         
         productRepo.Delete(existedProduct);
         await _unitOfWork.CompleteAsync();
         return true;
    }
}