namespace wema_test_service.Common.Models;

public sealed class AppSettings
{
    public string SmsBody { get; init; }
    public int OtpLength { get; init; }
    public int OtpValidity { get; init; }
    public string StatesEndpoint { get; init; }
    public string BanksEndpoint { get; init; }
    public CacheOptions CacheOptions { get; init; }
    public bool NotificationServiceEnabled { get; init; }
    public int NotificationServiceExecutionInterval { get; init; }
    public int SmsDispatchLimit { get; init; }
}

public sealed class CacheOptions
{
    public int AbsoluteExpiration { get; init; }
    public int SlidingExpiration { get; init; }
}
