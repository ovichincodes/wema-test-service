namespace wema_test_service.Data.Entities;

public class Otp : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
    public string SmsBody { get; set; }
    public OtpStatusEnum Status { get; set; }
    public DateTimeOffset? SentDate { get; set; }
    public DateTimeOffset? VerifiedTime { get; set; }
}
