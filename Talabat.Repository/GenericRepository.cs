using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyCollection<T>)await _dbContext.Set<Product>()
                                       .Include(p => p.Brand)
                                       .Include(p => p.Category)
                                       .ToListAsync();
            }

            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _dbContext.Set<Product>()
                                       .Where(p => p.Id == id)
                                       .Include(p => p.Brand)
                                       .Include(p => p.Category)
                                       .FirstOrDefaultAsync() as T;
            }

            return await _dbContext.Set<T>().FindAsync(id);
        }

        //----------------------------------------------------------------------------------------
        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task<T?> GetWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).ToListAsync();
        }



        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }


        public async Task AddAsync(T Item)
        {
            await _dbContext.Set<T>().AddAsync(Item);
        }

		public void Update(T entity)
        => _dbContext.Set<T>().Update(entity);

		public void Delete(T entity)
		=> _dbContext.Set<T>().Remove(entity);

	}
}
