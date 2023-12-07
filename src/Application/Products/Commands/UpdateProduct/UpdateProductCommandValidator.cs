using FluentValidation;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}