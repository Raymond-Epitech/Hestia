using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace EntityFramework.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAll(Guid colocationId);
    Task<List<T>> GetAllByList(List<Guid> idList);
    Task<List<TResult>> GetAllAsTypeAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<T?> GetByIdAsync(Guid id);
    Task<TResult?> GetByIdAsTypeAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector);
    Task<T> AddAsync(T entity);
    T Update(T entity);
    int UpdatRange(IEnumerable<T> entities);
    T Delete(T entity);
    Task<T> DeleteFromIdAsync(Guid id);
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
