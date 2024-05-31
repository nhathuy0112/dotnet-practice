using Application.Common.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetByIdAsync(command.CategoryId);

         if (category == null)
         {
             throw new ProductException("Cannot update product. Category is invalid");
         }

         var productRepo = _unitOfWork.ProductRepository;
         var existedProduct = await productRepo.GetByIdAsync(command.Id);
         
         if (existedProduct == null)
         {
             throw new ProductException("Cannot find product");
         }

         existedProduct.Name = command.Name;
         existedProduct.CategoryId = command.CategoryId;
         existedProduct.Price = command.Price;
         productRepo.Update(existedProduct);
         await _unitOfWork.CompleteAsync();
         return _mapper.Map<UpdateProductResponse>(existedProduct);
    }
}