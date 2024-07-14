namespace wema_test_service.Services.Interfaces;

public interface ICustomerService
{
    Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<PaginatedData<CustomerResponse>> GetCustomersAsync(string searchText, int pageSize, int pageNumber, CancellationToken cancellationToken = default);
}
