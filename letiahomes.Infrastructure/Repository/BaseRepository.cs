using letiahomes.Application.Abstractions.IRepository;
using letiahomes.Domain.Common;
using letiahomes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace letiahomes.Infrastructure.Repository
{
    public abstract class BaseRepository<T>:IBaseRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate ,bool trackChanges)=>
        
            !trackChanges?
                 _dbSet.Where(predicate).AsNoTracking():
                   _dbSet.Where(predicate);

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool>ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
