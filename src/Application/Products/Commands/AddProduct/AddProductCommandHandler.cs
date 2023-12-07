using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Products.Commands.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, AddProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AddProductResponse> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(command.CategoryId);
        
        if (category == null)
        {
            throw new ProductException("Cannot add product. Category is invalid");
        }
        
        var newProduct = new Product()
        {
            CategoryId = command.CategoryId,
            CreatedBy = command.CreatedBy,
            CreatedDate = DateTime.Now,
            Name = command.Name,
            Price = command.Price
        };
        await _unitOfWork.ProductRepository.AddProductAsync(newProduct);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<AddProductResponse>(newProduct);
    }
}