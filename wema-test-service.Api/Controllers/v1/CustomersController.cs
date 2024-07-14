namespace wema_test_service.Api.Controllers.v1;

public class CustomersController(ICustomerService customerService, IOtpService otpService) : BaseControllerV1
{
    private readonly ICustomerService _customerService = customerService;
    private readonly IOtpService _otpService = otpService;

    /// <summary>
    /// Onboard customer
    /// </summary>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("customers/onboard")]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        Guid customerId = await _customerService.CreateCustomerAsync(request.ToCustomer(), cancellationToken);
        return Ok(new BaseResponse<object>
        {
            ResponseData = new
            {
                customerId
            },
            ResponseMessage = "Customer created successfully. OTP has been sent your to phone number."
        });
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    /// <param name="request">Request param</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomersAsync([FromQuery] GetCustomerRequest request, CancellationToken cancellationToken = default)
    {
        PaginatedData<CustomerResponse> response = await _customerService.GetCustomersAsync(request.SearchText, request.PageSize, request.PageNumber, cancellationToken);
        return Ok(new BaseResponse<object>
        {
            ResponseData = response
        });
    }

    /// <summary>
    /// Resend OTP
    /// </summary>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("customers/otp/resend")]
    public async Task<IActionResult> ResendOtpAsync([FromBody] ResendOtpRequest request, CancellationToken cancellationToken = default)
    {
        await _otpService.ResendOtpAsync(request.CustomerId, cancellationToken);
        return Ok(new BaseResponse<object>
        {
            ResponseData = new
            {
                customerId = request.CustomerId
            },
            ResponseMessage = "OTP has been sent your to phone number."
        });
    }

    /// <summary>
    /// Validate OTP
    /// </summary>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("customers/otp/validate")]
    public async Task<IActionResult> ValidateOtpAsync([FromBody] ValidateOtpRequest request, CancellationToken cancellationToken = default)
    {
        await _otpService.ValidateOtpAsync(request.CustomerId, request.Code, cancellationToken);
        return Ok(new BaseResponse<object>
        {
            ResponseData = "OTP verified. Customer onboarding successful"
        });
    }
}
