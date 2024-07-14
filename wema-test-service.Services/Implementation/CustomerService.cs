namespace wema_test_service.Services.Implementation;

public sealed class CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger, IOptions<AppSettings> options, IOtpService otpService, IUtilityService utilityService) : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IOtpService _otpService = otpService;
    private readonly IUtilityService _utilityService = utilityService;
    private readonly ILogger<CustomerService> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;

    public async Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"CUSTOMER_SERVICE__{nameof(CreateCustomerAsync)} => Process started...");

        if (await _unitOfWork.CustomerRepository.AnyAsync(s => s.PhoneNumber.Equals(customer.PhoneNumber), cancellationToken))
            throw new BadRequestException("Phone number already exist");

        // validate lga to state mapping
        IEnumerable<string> states = await _utilityService.GetStatesAsync(cancellationToken);
        if (!states.Any(s => s.ToLower().Trim().Equals(customer.StateOfResidence.ToLower())))
            throw new BadRequestException("Invalid State");

        IEnumerable<string> lgas = await _utilityService.GetStateLgasAsync(customer.StateOfResidence, cancellationToken);
        if (!lgas.Any(s => s.ToLower().Trim().Equals(customer.Lga.ToLower())))
            throw new BadRequestException("Invalid LGA");

        IExecutionStrategy executionStrategy = _unitOfWork.CreateExecutionStrategy();
        Guid result = await executionStrategy.ExecuteAsync(async () =>
        {
            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                customer.Password = UtilityHelper.HashText(customer.Password);
                Guid customerId = await _unitOfWork.CustomerRepository.InsertAsync(customer, cancellationToken);

                await _otpService.CreateOtpAsync(customerId, customer.PhoneNumber, false, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"CUSTOMER_SERVICE__{nameof(CreateCustomerAsync)} => OTP generated...");
                return customerId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, $"CUSTOMER_SERVICE__{nameof(CreateCustomerAsync)} => An error occured while trying to create customer.");
                throw new BaseException(System.Net.HttpStatusCode.InternalServerError, "An error occured while creating customer");
            }
        });
        return result;
    }

    public async Task<PaginatedData<CustomerResponse>> GetCustomersAsync(string searchText, int pageSize, int pageNumber, CancellationToken cancellationToken = default)
    {
        searchText = searchText?.Trim()?.ToString();

        IQueryable<Customer> iCustomers = _unitOfWork.CustomerRepository.Get(s => null == searchText
            || s.Email.ToLower().Contains(searchText)
            || s.Lga.ToLower().Contains(searchText)
            || s.PhoneNumber.ToLower().Contains(searchText)
            || s.StateOfResidence.ToLower().Contains(searchText))
            .AsNoTracking().OrderByDescending(s => s.CreatedDate);

        IEnumerable<CustomerResponse> customers = await iCustomers.Select(s => new CustomerResponse
        {
            CreatedDate = s.CreatedDate,
            CreatedDateFormatted = s.CreatedDate.ToString("MMMM d, yyyy"),
            CreatedTimeFormatted = s.CreatedDate.ToString("hh:mm tt"),
            Email = s.Email,
            Id = s.Id,
            Lga = s.Lga,
            PhoneNumber = s.PhoneNumber,
            StateOfResidence = s.StateOfResidence,
            Status = s.Status,
            VerificationStatus = s.VerificationStatus
        }).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        int totalRecordsCount = await iCustomers.CountAsync(cancellationToken);

        return new PaginatedData<CustomerResponse>(customers, totalRecordsCount, pageNumber, pageSize);
    }
}
