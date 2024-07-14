namespace wema_test_service.Services.Interfaces;

public interface ISmsService
{
    Task ProcessUnsentSmsAsync();
    Task<bool> SendSmsAsync(Otp otp);
}
