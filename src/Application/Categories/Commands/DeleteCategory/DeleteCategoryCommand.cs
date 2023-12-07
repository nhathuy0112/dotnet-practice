using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<bool>
{
    public DeleteCategoryCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}