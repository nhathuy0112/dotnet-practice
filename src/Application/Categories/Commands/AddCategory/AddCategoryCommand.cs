using MediatR;

namespace Application.Categories.Commands.AddCategory;

public class AddCategoryCommand : IRequest<AddCategoryResponse>
{
    public string Name { get; set; }
}