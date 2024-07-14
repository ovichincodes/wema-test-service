using Microsoft.EntityFrameworkCore.Storage;

namespace wema_test_service.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Customer> CustomerRepository { get; }
    IRepository<Otp> OtpRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    IExecutionStrategy CreateExecutionStrategy();
}
