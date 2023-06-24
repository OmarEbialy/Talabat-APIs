using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        // Ask CLR to Make object From Class DbContext Implicity
        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region Static Queries
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
                return (IReadOnlyList<T>)await _dbContext.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
            else
                return await _dbContext.Set<T>().ToListAsync();
        }


        public async Task<T> GetByIdAsync(int id)
        {
            //return await _dbContext.Set<T>().Where(X => X.Id == id).FirstOrDefault();
            return await _dbContext.Set<T>().FindAsync(id);
        }
        #endregion

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T> GetEntityWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task Add(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
    }
}
