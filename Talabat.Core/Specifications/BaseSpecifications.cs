using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; set; } = null; //hold expression to send it to Where()
                                                                         // P => P.Id == id
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; } = null;
        public Expression<Func<T, object>> OrderByDesc { get; set; } = null;
        public int Take { get; set; } = 0;
        public int Skip { get; set; } = 0;
        public bool IsPaginationEnabled { get; set; } = false;

		public void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
		{
			OrderByDesc = orderByDescExpression;
		}
		public BaseSpecifications() {}
        public BaseSpecifications(Expression<Func<T, bool>>? CriteriaExpression)
        {
            Criteria = CriteriaExpression;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;
        }

    }

    
}
