namespace wema_test_service.Api.Requests.RequestValidators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(s => s.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(s => s.PhoneNumber)
        .NotEmpty().WithMessage("Phone number is required.")
        .Matches(@"^(234|0)[789]0\d{8}$")
        .WithMessage("Invalid Nigerian phone number format.");

        RuleFor(s => s.Lga)
        .NotEmpty().WithMessage("LGA is required");

        RuleFor(s => s.StateOfResidence)
        .NotEmpty().WithMessage("StateOfResidence is required");

        RuleFor(s => s.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
        .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
        .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
        .Matches(@"[\!\@\#\$\%\^\&\*\(\)\+\-]").WithMessage("Password must contain at least one special character.");

        RuleFor(s => s.ConfirmPassword)
        .NotEmpty().WithMessage("Confirm Password is required.")
        .Equal(s => s.Password).WithMessage("Passwords do not match.");
    }
}
