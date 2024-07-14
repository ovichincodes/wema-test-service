namespace wema_test_service.Data.Entities;

public class Customer : BaseEntity
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string StateOfResidence { get; set; }
    public string Lga { get; set; }
    public CustomerStatusEnum Status { get; set; }
    public CustomerVerificationStatusEnum VerificationStatus { get; set; }
}
