using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace EntityFramework.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query(bool asNoTracking = true);
    Task<List<T>> GetAllAsync(Guid colocationId);
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task<int> AddRangeAsync(IEnumerable<T> entities);
    T Update(T entity);
    int UpdateRange(IEnumerable<T> entities);
    T Delete(T entity);
    Task<T> DeleteFromIdAsync(Guid id);
    int DeleteRangeAsync(IEnumerable<T> entities);
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
