
using Microsoft.EntityFrameworkCore;
using TequlaisRestaurant.Data;

namespace TequlaisRestaurant.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _dbContext { get; set; }
        private DbSet<T> _dbSet {  get; set; }

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet =  dbContext.Set<T>();
                
        }
        public Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(int id, QueryOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
