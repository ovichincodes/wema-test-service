namespace wema_test_service.Services.Interfaces;

public interface IOtpService
{
    Task CreateOtpAsync(Guid customerId, string phoneNumber, bool useExecutionStrategy = true, CancellationToken cancellationToken = default);
    Task ResendOtpAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task ValidateOtpAsync(Guid trackingId, string code, CancellationToken cancellationToken = default);
}
