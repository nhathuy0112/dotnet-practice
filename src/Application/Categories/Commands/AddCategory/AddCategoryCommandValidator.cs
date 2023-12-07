using FluentValidation;

namespace Application.Categories.Commands.AddCategory;

public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
{
    public AddCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}