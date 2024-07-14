namespace wema_test_service.Data.Repositories.Implementation;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context = context;
    private readonly DbSet<T> _entities = context.Set<T>();

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        => await _entities.AnyAsync(expression, cancellationToken);

    public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        => _entities.AsNoTracking().Where(expression);

    public IEnumerable<T> GetWhere(Expression<Func<T, bool>> expression)
        => _entities.Where(expression);

    public async Task<T> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _entities.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<T> GetSingleWhereAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        => await _entities.Where(expression).FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _entities.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<(int totalRecordsCount, IEnumerable<T>)> Paginate(Expression<Func<T, bool>> expression, int skip, int take)
    {
        IQueryable<T> iEntities = _entities.Where(expression).AsNoTracking().OrderByDescending(s => s.CreatedDate);
        IQueryable<T> entities = iEntities.Skip((skip - 1) * take).Take(take);
        int totalRecordsCount = await entities.CountAsync();
        return (totalRecordsCount, entities);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
