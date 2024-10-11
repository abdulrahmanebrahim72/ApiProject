using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    internal static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> queryInput, ISpecification<TEntity> spec)
        {
            var query = queryInput; //_dbContext.Set<TEntity>()
            
            if(spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }
            //_dbContext.Set<Product>().Where(p => p.Id == id)
            //include Expressions
            //1. p => p.Brand
            //2. p => p.Category
            //3. ......


            if(spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDesc is not null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }

            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }


            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

            //query = _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Brand)
            //query = _dbContext.Set<Product>().Where(p => p.Id == id).Include(p => p.Brand).Include(p => p.Category)


            return query;
        }
    }
}
