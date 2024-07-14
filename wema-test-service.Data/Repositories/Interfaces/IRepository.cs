namespace wema_test_service.Data.Repositories.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<Guid> InsertAsync(T entity, CancellationToken cancellationToken = default);
    IQueryable<T> Get(Expression<Func<T, bool>> expression);
    IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression);
    Task<T> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<T> GetSingleWhereAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}
