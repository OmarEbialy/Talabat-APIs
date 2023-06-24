using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        #region Static Queries
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        #endregion

        #region Dynamic Quries Using SpecificationDesignPattern
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);

        #endregion

        // Pagination Count
        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);

        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
