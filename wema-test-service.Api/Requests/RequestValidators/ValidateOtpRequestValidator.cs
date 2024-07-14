namespace wema_test_service.Api.Requests.RequestValidators;

public class ValidateOtpRequestValidator : AbstractValidator<ValidateOtpRequest>
{
    public ValidateOtpRequestValidator()
    {
        RuleFor(s => s.CustomerId).NotEmpty().WithMessage("CustomerId is required");
        RuleFor(s => s.Code).NotEmpty().WithMessage("Code is required").Length(6).WithMessage("Code must be 6 digits").Must(x => x.All(v => char.IsDigit(v))).WithMessage("Code must contain only valid digits");
    }
}

public class ResendOtpRequestValidator : AbstractValidator<ResendOtpRequest>
{
    public ResendOtpRequestValidator()
    {
        RuleFor(s => s.CustomerId).NotEmpty().WithMessage("CustomerId is required");
    }
}
