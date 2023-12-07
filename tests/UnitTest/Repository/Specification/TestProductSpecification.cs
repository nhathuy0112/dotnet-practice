using Domain.Entities;
using Domain.Specification;

namespace UnitTest.Repository.Specification;

public class TestProductSpecification
{
    internal class TestProductByNameSpec : SpecificationBase<Product>
    {
        public TestProductByNameSpec(string name)
        {
            Criteria = p => p.Name.ToLower().Contains(name.ToLower());
        }
    }
    
    internal class TestProductOrderByNameSpec : SpecificationBase<Product>
    {
        public TestProductOrderByNameSpec()
        {
            AddOrderBy(p => p.Name);
        }
    }
    
    internal class TestProductOrderByIdDescSpec : SpecificationBase<Product>
    {
        public TestProductOrderByIdDescSpec()
        {
            AddOrderByDescending(p => p.Id);
        }
    }
    
    internal class TestProductIncludeCategorySpec : SpecificationBase<Product>
    {
        public TestProductIncludeCategorySpec()
        {
            AddInclude(p => p.Category);
        }
    }
    
    internal class TestProductWithPaginationSpec : SpecificationBase<Product>
    {
        public TestProductWithPaginationSpec(int skip, int take)
        {
            ApplyPaging(skip, take);
        }
    }

    internal class TestProductForCountSpec : SpecificationBase<Product>
    {
        public TestProductForCountSpec()
        {
        }

        public TestProductForCountSpec(string name)
        {
            Criteria = p => p.Name == name;
        }
    }
}