using Application.Categories.MappingProfile;
using Application.Products.MappingProfile;
using Application.Users.MappingProfile;
using AutoMapper;

namespace API.Extensions;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection service)
    {
        service.AddAutoMapper(config =>
        {
            var profiles = new List<Profile>()
            {
                new CategoryProfile(),
                new ProductProfile(),
                new UserProfile()
            };
            
            config.AddProfiles(profiles);
        });
        return service;
    }
}