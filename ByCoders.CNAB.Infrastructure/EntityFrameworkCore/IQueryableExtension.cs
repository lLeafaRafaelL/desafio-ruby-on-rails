using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore;

public static class IQueryableExtension
{
    public static IQueryable<TEntity> ApplyIncludes<TEntity>(this IQueryable<TEntity> queryable, bool asNotTracking = false, bool asSplitQuery = false, params Expression<Func<TEntity, object>>[] includes) where TEntity : class
    {
        if (includes == null)
            return queryable;

        var objectQuery = queryable as IQueryable<TEntity>;

        if (objectQuery != null)
        {
            foreach (var include in includes)
                objectQuery = objectQuery.Include(include);

            if (asNotTracking)
                objectQuery = objectQuery.AsNoTracking();

            if (asSplitQuery)
                objectQuery = objectQuery.AsSplitQuery();

            return objectQuery;
        }

        return queryable;
    }

    public static IQueryable<TEntity> ApplyPredicates<TEntity>(this IQueryable<TEntity> queryable, params Expression<Func<TEntity, bool>>[] predicates) where TEntity : class
    {
        if (predicates == null)
            return queryable;

        var objectQuery = queryable as IQueryable<TEntity>;

        if (objectQuery != null)
        {
            foreach (var predicate in predicates)
                objectQuery = objectQuery.Where(predicate);

            return objectQuery;
        }


        return queryable;
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }
}