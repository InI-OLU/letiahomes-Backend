using letiahomes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Application.Abstractions.IRepository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Attach(T entity);
        void Delete(T entity);
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> Entry(T entity);
        Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(System.Linq.Expressions.Expression<Func<T, bool>> predicate, bool trackChanges);
        Task<T?> GetByIdAsync(Guid id);
        void Update(T entity);
    }
}
