using Application.Common.Helpers;
using Domain.QueryParams.User;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUserQuery : IRequest<PaginatedResponse<GetUsersResponse>>
{
    public GetUserQuery(UserQueryParams queryParams)
    {
        QueryParams = queryParams;
    }

    public UserQueryParams QueryParams { get; }
}