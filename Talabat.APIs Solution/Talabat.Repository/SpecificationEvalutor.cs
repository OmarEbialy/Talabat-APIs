using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvalutor<TEntity> where TEntity : BaseEntity
    {
        //                                                  dbContex                    specification
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery , ISpecification<TEntity> spec)
        {
            var query = inputQuery; // quey = _dbContext.Products
            // function 5asa b elly feha WhereCondition
            if (spec.Criteria is not null) // P => P.Id == 1
                query = query.Where(spec.Criteria);
            // query = _dbContext.Products.Where(P => P.Id == 1)

            // Query For Sorting
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            // Query for Pagination
            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            if (spec.OrderByDecsending != null)
                query = query.OrderByDescending(spec.OrderByDecsending);





            // Includes 
            //      1. P => P.ProductBrand
            //      2. P => P.ProductType
            // query = _dbContext.Products.Where(P => P.Id == 1).Include(P => P.ProductBrand).Include(P => P.ProductType)
            // currentQuery refer to query

            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            return query;
        }
    }
}
