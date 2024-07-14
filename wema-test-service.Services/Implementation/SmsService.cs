namespace wema_test_service.Services.Implementation;

public class SmsService(IUnitOfWork unitOfWork, ILogger<OtpService> logger, IOptions<AppSettings> options, IServiceScopeFactory serviceScopeFactory) : ISmsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<OtpService> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task ProcessUnsentSmsAsync()
    {
        _logger.LogInformation($"SMS_SERVICE__{nameof(ProcessUnsentSmsAsync)} => Process started...");

        IEnumerable<Otp> unSentOtps = _unitOfWork.OtpRepository.GetWhere(s => s.Status == OtpStatusEnum.Created && s.SentDate == null).Take(_appSettings.SmsDispatchLimit);

        await Parallel.ForEachAsync(unSentOtps, async (otp, cancellationToken) =>
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IRepository<Otp> otpRepo = scope.ServiceProvider.GetRequiredService<IRepository<Otp>>();
            ISmsService smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();

            bool isSent = await smsService.SendSmsAsync(otp);
            if (isSent)
            {
                otp.Status = OtpStatusEnum.Sent;
                otp.SentDate = DateTimeOffset.UtcNow;
                otp.ModifiedDate = DateTimeOffset.UtcNow;

                await otpRepo.UpdateAsync(otp, cancellationToken);
                _logger.LogInformation($"SMS_SERVICE__{nameof(ProcessUnsentSmsAsync)} => Otp sent. Id - {otp.Id}");
            }
        });
    }

    public Task<bool> SendSmsAsync(Otp otp)
    {
        // implement send sms logic

        return Task.FromResult(true);
    }
}
