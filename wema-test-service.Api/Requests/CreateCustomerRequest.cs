namespace wema_test_service.Api.Requests;

public record CreateCustomerRequest
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string StateOfResidence { get; set; }
    public string Lga { get; set; }

    public Customer ToCustomer()
    {
        return new Customer
        {
            CreatedDate = DateTimeOffset.UtcNow,
            Email = Email.Trim(),
            Lga = Lga.Trim(),
            ModifiedDate = DateTimeOffset.UtcNow,
            Password = Password,
            PhoneNumber = PhoneNumber.Trim(),
            StateOfResidence = StateOfResidence.Trim(),
            Status = CustomerStatusEnum.Created,
            VerificationStatus = CustomerVerificationStatusEnum.Pending
        };
    }
}

public record ResendOtpRequest
{
    public Guid CustomerId { get; set; }
}

public record ValidateOtpRequest : ResendOtpRequest
{
    public string Code { get; set; }
}
