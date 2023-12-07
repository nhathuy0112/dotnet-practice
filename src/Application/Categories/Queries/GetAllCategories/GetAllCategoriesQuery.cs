using MediatR;

namespace Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryResponse>> {}
