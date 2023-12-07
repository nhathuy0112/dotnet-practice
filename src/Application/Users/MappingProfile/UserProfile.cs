using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.GetUsers;
using AutoMapper;
using Domain.Entities;

namespace Application.Users.MappingProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, GetUsersResponse>();
        CreateMap<AppUser, UpdateUserResponse>();
    }
}