namespace wema_test_service.Services.Implementation;

public sealed class UtilityService(IHttpClientService httpClientService, IOptions<AppSettings> options, IMemoryCache memoryCache) : IUtilityService
{
    private readonly IHttpClientService _httpClientService = httpClientService;
    private readonly AppSettings _appSettings = options.Value;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<BanksResponse> GetBanksAsync(CancellationToken cancellationToken = default)
    {
        bool isBanksCached = _memoryCache.TryGetValue("banks", out BanksResponse banks);
        if (!isBanksCached)
        {
            (BanksResponse successfulResponse, dynamic failedResponse) = await _httpClientService.GetAsync<BanksResponse, dynamic>(_appSettings.BanksEndpoint, cancellationToken);

            if (failedResponse is not null)
                throw new Exception("An error occured while trying to fetch banks.");

            if (successfulResponse is null || successfulResponse.HasError || !successfulResponse.Result.Any())
                throw new Exception($"Banks not found");

            // set lga cache
            double absoluteExpiration = Convert.ToDouble(_appSettings.CacheOptions.AbsoluteExpiration);
            double slidingExpiration = Convert.ToDouble(_appSettings.CacheOptions.SlidingExpiration);
            MemoryCacheEntryOptions options = new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(absoluteExpiration),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = TimeSpan.FromDays(slidingExpiration)
            };
            banks = successfulResponse;
            _memoryCache.Set("banks", banks, options);
        }
        return banks;
    }

    public async Task<IEnumerable<string>> GetStateLgasAsync(string state, CancellationToken cancellationToken = default)
    {
        (IEnumerable<string> successfulResponse, dynamic failedResponse) = await _httpClientService.GetAsync<IEnumerable<string>, dynamic>($"{_appSettings.StatesEndpoint}?state={state}", cancellationToken);

        if (failedResponse is not null)
            throw new Exception("An error occured while trying to fetch LGAs.");

        if (successfulResponse is null || !successfulResponse.Any())
            throw new Exception($"LGAs not found");

        return successfulResponse;
    }

    public async Task<IEnumerable<string>> GetStatesAsync(CancellationToken cancellationToken = default)
    {
        bool isStatesCached = _memoryCache.TryGetValue("states", out IEnumerable<string> states);
        if (!isStatesCached)
        {
            (IEnumerable<string> successfulResponse, dynamic failedResponse) = await _httpClientService.GetAsync<IEnumerable<string>, dynamic>($"{_appSettings.StatesEndpoint}fetch", cancellationToken);

            if (failedResponse is not null)
                throw new Exception("An error occured while trying to fetch states.");

            if (successfulResponse is null || !successfulResponse.Any())
                throw new Exception($"States not found");

            // set states cache
            double absoluteExpiration = Convert.ToDouble(_appSettings.CacheOptions.AbsoluteExpiration);
            double slidingExpiration = Convert.ToDouble(_appSettings.CacheOptions.SlidingExpiration);
            MemoryCacheEntryOptions options = new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(absoluteExpiration),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromDays(slidingExpiration)
            };
            states = successfulResponse;
            _memoryCache.Set("states", states, options);
        }
        return states;
    }
}
