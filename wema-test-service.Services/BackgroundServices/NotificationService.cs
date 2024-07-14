using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace wema_test_service.Services.BackgroundServices;

public class NotificationService : IHostedService, IDisposable
{
    private readonly ILogger<NotificationService> _logger;
    private readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private long executionCount = 0;
    private Timer _timer;

    public NotificationService(ILogger<NotificationService> logger,
    IOptionsMonitor<AppSettings> appSettingsMonitor,
    IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _appSettings = appSettingsMonitor.CurrentValue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification background service started running.");

        int interval = _appSettings.NotificationServiceExecutionInterval;
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
        TimeSpan.FromMinutes(interval));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        if (_appSettings.NotificationServiceEnabled)
        {
            _ = DoWorkAsync(state);
        }
    }
    private async Task DoWorkAsync(object state)
    {
        long count = Interlocked.Increment(ref executionCount);
        _logger.LogInformation("Notification background service started executing task {Count}", count);
        try
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();
            await smsService.ProcessUnsentSmsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        finally
        {
            _logger.LogInformation("Notification background service completed task {Count}", count);
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification background service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
