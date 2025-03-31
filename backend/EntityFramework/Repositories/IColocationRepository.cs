using EntityFramework.Models;
using System.Linq.Expressions;

namespace EntityFramework.Repositories;

public interface IColocationRepository<T> where T : class, IColocationEntity
{
    Task<List<T>> GetAllByColocationIdAsync(Guid colocationId);
    Task<List<TResult>> GetAllByColocationIdAsTypeAsync<TResult>(Guid colocationId, Expression<Func<T, TResult>> selector);
}
