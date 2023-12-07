using Application.Common.Helpers;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUserQuery, PaginatedResponse<GetUsersResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<PaginatedResponse<GetUsersResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var queryParams = query.QueryParams;
        var userRole = await _userRepository.GetRoleAsync(Role.User);
        var count = await _userRepository.CountUsersAsync(queryParams, userRole.Id);
        var users = await _userRepository.GetUsersAsync(queryParams, userRole.Id);
        var data = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<GetUsersResponse>>(users);
        return new PaginatedResponse<GetUsersResponse>()
        {
            PageIndex = queryParams.PageIndex,
            PageSize = queryParams.PageSize,
            Count = count,
            Data = data
        };
    }
}