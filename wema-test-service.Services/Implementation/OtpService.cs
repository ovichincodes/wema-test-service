namespace wema_test_service.Services.Implementation;

public class OtpService(IUnitOfWork unitOfWork, ILogger<OtpService> logger, IOptions<AppSettings> options) : IOtpService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<OtpService> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;

    private async Task DoCreateOtpAsync(Guid customerId, string phoneNumber, bool isResend = false, CancellationToken cancellationToken = default)
    {
        Otp existingOtp = await _unitOfWork.OtpRepository.GetSingleWhereAsync(s => s.CustomerId == customerId && (s.Status == OtpStatusEnum.Created || s.Status == OtpStatusEnum.Sent), cancellationToken);
        if (existingOtp is not null)
        {
            existingOtp.ModifiedDate = DateTimeOffset.UtcNow;
            existingOtp.Status = OtpStatusEnum.Invalidated;
            await _unitOfWork.OtpRepository.UpdateAsync(existingOtp, cancellationToken);
        }

        phoneNumber = isResend ? existingOtp.PhoneNumber : phoneNumber;
        string code = UtilityHelper.GenerateOtp(_appSettings.OtpLength);
        string smsBody = _appSettings.SmsBody.Replace("[OTP]", code);
        existingOtp = new Otp
        {
            Code = UtilityHelper.HashText(code),
            CreatedDate = DateTimeOffset.UtcNow,
            CustomerId = customerId,
            ModifiedDate = DateTimeOffset.UtcNow,
            PhoneNumber = phoneNumber,
            SmsBody = smsBody,
            Status = OtpStatusEnum.Created
        };

        await _unitOfWork.OtpRepository.InsertAsync(existingOtp, cancellationToken);
    }

    public async Task CreateOtpAsync(Guid customerId, string phoneNumber, bool useExecutionStrategy = true, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"OTP_SERVICE__{nameof(CreateOtpAsync)} => Process started...");

        if (!useExecutionStrategy)
        {
            await DoCreateOtpAsync(customerId, phoneNumber, false, cancellationToken);
            _logger.LogInformation($"OTP_SERVICE__{nameof(CreateOtpAsync)} => OTP generated...");
            return;
        }

        IExecutionStrategy executionStrategy = _unitOfWork.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await DoCreateOtpAsync(customerId, phoneNumber, false, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"OTP_SERVICE__{nameof(CreateOtpAsync)} => OTP generated...");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, $"OTP_SERVICE__{nameof(CreateOtpAsync)} => An error occured while trying to generate otp.");
                throw new Exception("An error occured while sending otp");
            }
        });
    }

    public async Task ResendOtpAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"OTP_SERVICE__{nameof(ResendOtpAsync)} => Process started...");

        IExecutionStrategy executionStrategy = _unitOfWork.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await DoCreateOtpAsync(customerId, "", true, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"OTP_SERVICE__{nameof(ResendOtpAsync)} => OTP resent...");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, $"OTP_SERVICE__{nameof(ResendOtpAsync)} => An error occured while trying to generate otp.");
                throw new Exception("An error occured while sending otp");
            }
        });
    }

    public async Task ValidateOtpAsync(Guid customerId, string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"OTP_SERVICE__{nameof(ValidateOtpAsync)} => Process started...");

        Otp otp = await _unitOfWork.OtpRepository.GetSingleWhereAsync(s => s.CustomerId == customerId && s.Status == OtpStatusEnum.Sent, cancellationToken) ?? throw new BadRequestException("Invalid OTP");

        if (otp.Status == OtpStatusEnum.Verified)
            throw new BadRequestException("OTP has been verified");

        if (otp.Status == OtpStatusEnum.Invalidated)
            throw new BadRequestException("Invalid OTP");

        IExecutionStrategy executionStrategy = _unitOfWork.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                double otpValidity = Convert.ToDouble(_appSettings.OtpValidity);
                if (DateTimeOffset.UtcNow.Subtract(otp.CreatedDate).TotalMinutes > otpValidity)
                {
                    otp.Status = OtpStatusEnum.Expired;
                    otp.ModifiedDate = DateTimeOffset.UtcNow;
                    await _unitOfWork.OtpRepository.UpdateAsync(otp, cancellationToken);
                    throw new BadRequestException("OTP expired");
                }

                bool isOtpValid = UtilityHelper.VerifyHashedText(otp.Code, code);
                if (!isOtpValid)
                    throw new BadRequestException("Invalid OTP");

                otp.Status = OtpStatusEnum.Verified;
                otp.ModifiedDate = DateTimeOffset.UtcNow;
                otp.VerifiedTime = DateTimeOffset.UtcNow;
                await _unitOfWork.OtpRepository.UpdateAsync(otp, cancellationToken);

                Customer customer = await _unitOfWork.CustomerRepository.GetById(otp.CustomerId, cancellationToken);
                if (customer is null)
                {
                    _logger.LogError($"OTP_SERVICE__{nameof(ValidateOtpAsync)} => Customer record not found.");
                    throw new Exception("An error occured while validating OTP");
                }

                customer.ModifiedDate = DateTimeOffset.UtcNow;
                customer.Status = CustomerStatusEnum.Active;
                customer.VerificationStatus = CustomerVerificationStatusEnum.Verified;
                await _unitOfWork.CustomerRepository.UpdateAsync(customer, cancellationToken);

                await _unitOfWork.CompleteAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation($"OTP_SERVICE__{nameof(ValidateOtpAsync)} => OTP verified...");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, $"OTP_SERVICE__{nameof(ValidateOtpAsync)} => An error occured while trying to validate otp.");
                throw new Exception("An error occured while validating otp");
            }
        });
    }
}
