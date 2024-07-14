namespace wema_test_service.Services.Interfaces;

public interface IUtilityService
{
    Task<IEnumerable<string>> GetStatesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetStateLgasAsync(string state, CancellationToken cancellationToken = default);
    Task<BanksResponse> GetBanksAsync(CancellationToken cancellationToken = default);
}
