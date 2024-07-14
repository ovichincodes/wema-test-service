using Microsoft.EntityFrameworkCore.Storage;

namespace wema_test_service.Data.Repositories.Implementation;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    public IRepository<Customer> CustomerRepository { get; private set; }
    public IRepository<Otp> OtpRepository { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        CustomerRepository = new Repository<Customer>(_context);
        OtpRepository = new Repository<Otp>(_context);
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _context.Database.CreateExecutionStrategy();
    }
}
