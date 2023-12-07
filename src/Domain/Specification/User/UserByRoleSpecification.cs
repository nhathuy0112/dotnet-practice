using Domain.Entities;

namespace Domain.Specification.User;

public class UserByRoleSpecification : SpecificationBase<AppUser>
{
    public UserByRoleSpecification(string roleId, UserRequestParams requestParams)
    {
        Criteria = u => u.UserRoles.Any(r => r.RoleId == roleId)
                        && (string.IsNullOrEmpty(requestParams.Email) ||
                            u.Email.ToLower().Contains(requestParams.Email.ToLower()));
        AddInclude(u => u.UserRoles);
        ApplyPaging(requestParams.PageSize * (requestParams.PageIndex -1), requestParams.PageSize);

        if (!string.IsNullOrEmpty(requestParams.Sort))
        {
            switch (requestParams.Sort)
            {
                case "email":
                    AddOrderBy(u => u.Email);
                    break;
                case "email-desc":
                    AddOrderByDescending(u => u.Email);
                    break;
                default:
                    AddOrderBy(u => u.Id);
                    break;
            }
        }
    }
}