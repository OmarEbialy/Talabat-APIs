using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecification<T> where T : BaseEntity
    {
        // Property to WhereCondition
        public Expression<Func<T, bool>> Criteria { get; set; }

        // Property to Include()
        public List<Expression<Func<T, object>>> Includes { get; set; }

        // Property to Sort OrderBy
        public Expression<Func<T , object>> OrderBy { get; set; }

        // Property to Sort OrderByDecsending
        public Expression<Func<T , object>> OrderByDecsending { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }
    }
}
