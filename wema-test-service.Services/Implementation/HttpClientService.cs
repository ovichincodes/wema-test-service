namespace wema_test_service.Services.Implementation;

public sealed class HttpClientService(IHttpClientFactory httpClientFactory) : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<(TResponse successfulResponse, T failedResponse)> GetAsync<TResponse, T>(string endpoint, CancellationToken cancellationToken = default)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(endpoint, cancellationToken);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            T failedResponse = await HandleResponseAsync<T>(httpResponseMessage, cancellationToken);
            return (default, failedResponse);
        }

        TResponse response = await HandleResponseAsync<TResponse>(httpResponseMessage, cancellationToken);
        return (response, default);
    }

    private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default)
    {
        using Stream contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);

        string responseContent = await new StreamReader(contentStream).ReadToEndAsync(cancellationToken);
        T response = UtilityHelper.DeSerializer<T>(responseContent);

        return response;
    }
}
