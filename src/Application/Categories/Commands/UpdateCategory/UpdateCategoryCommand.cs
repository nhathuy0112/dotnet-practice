using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<UpdateCategoryResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
}