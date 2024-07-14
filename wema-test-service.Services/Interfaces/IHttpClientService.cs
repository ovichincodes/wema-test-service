namespace wema_test_service.Services.Interfaces;

public interface IHttpClientService
{
    Task<(TResponse successfulResponse, T failedResponse)> GetAsync<TResponse, T>(string endpoint, CancellationToken cancellationToken = default);
}
