using FluentValidation;

namespace Application.Dto.Product;

public class ProductRequestValidation : AbstractValidator<ProductRequest>
{
    public ProductRequestValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
public class ProductRequest
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int CategoryId { get; set; }
}