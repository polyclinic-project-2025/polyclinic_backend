using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;   

namespace PolyclinicDomain.IRepositories;

public interface IRepository<T> where T : class
{
    // CREATE
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    // READ
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // UPDATE
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);

    // DELETE
    Task DeleteAsync(T entity);
    Task DeleteByIdAsync(string id);
    Task DeleteRangeAsync(IEnumerable<T> entities);

    // Others
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}