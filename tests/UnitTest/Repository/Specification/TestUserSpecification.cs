using Domain.Entities;
using Domain.Specification;

namespace UnitTest.Repository.Specification;

public class TestUserSpecification
{
    internal class TestAllUserSpec : SpecificationBase<AppUser> {}
    
    internal class TestUserSpecWithFilter : SpecificationBase<AppUser>
    {
        public TestUserSpecWithFilter(string email)
        {
            Criteria = u => u.Email.ToLower().Contains(email.ToLower());
        }
    }

    internal class TestUserSpecOrderBy : SpecificationBase<AppUser>
    {
        public TestUserSpecOrderBy()
        {
            AddOrderBy(u => u.UserName);
        }
    }
}