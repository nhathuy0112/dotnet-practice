using Microsoft.EntityFrameworkCore;

namespace Domain.Specification;

public class SpecificationEvaluator<T> where T : class
{
    private static IQueryable<T> ApplyCriteria(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.Criteria != null)
        {
            query = query
                .Where(specification.Criteria);
        }

        return query;
    }

    private static IQueryable<T> ApplyOrderBy(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        
        return query;
    }
    
    private static IQueryable<T> ApplyOrderByDescending(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }
        
        return query;
    }

    private static IQueryable<T> ApplyPaging(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.IsPagingEnabled)
        {
            query = query
                .Skip(specification.Skip)
                .Take(specification.Take);
        }

        return query;
    }

    private static IQueryable<T> ApplyInclude(IQueryable<T> query, ISpecification<T> specification)
    {
        if (specification.Includes.Count > 0)
        {
            query = specification.Includes.Aggregate(query, (current, include) =>
                current.Include(include)
            );
            if (specification.Includes.Count >= 3) query.AsSplitQuery();
        }

        return query;
    }
    
    public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> specification)
    {
        query = ApplyCriteria(query, specification);
        query = ApplyOrderBy(query, specification);
        query = ApplyOrderByDescending(query, specification);
        query = ApplyPaging(query, specification);
        query = ApplyInclude(query, specification);
        return query;
    }
}