using wema_test_service.Common.Enums;

namespace wema_test_service.Common.Models;

public record CustomerResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string StateOfResidence { get; set; }
    public string Lga { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string CreatedDateFormatted { get; set; }
    public string CreatedTimeFormatted { get; set; }
    public CustomerStatusEnum Status { get; set; }
    public CustomerVerificationStatusEnum VerificationStatus { get; set; }
}
