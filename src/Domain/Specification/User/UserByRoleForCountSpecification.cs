using Domain.Entities;

namespace Domain.Specification.User;

public class UserByRoleForCountSpecification : SpecificationBase<AppUser>
{
    public UserByRoleForCountSpecification(string roleId, UserRequestParams requestParams)
    {
        Criteria = u => u.UserRoles.Any(r => r.RoleId == roleId)
                        && (string.IsNullOrEmpty(requestParams.Email) ||
                            u.Email.ToLower().Contains(requestParams.Email.ToLower()));
        AddInclude(u => u.UserRoles);
    }
}