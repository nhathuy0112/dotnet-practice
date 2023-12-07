using FluentValidation;

namespace Application.Dto.Category;

public class CategoryRequestValidation : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidation()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
public class CategoryRequest
{
    public string Name { get; set; }
}