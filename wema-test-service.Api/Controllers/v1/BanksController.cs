namespace wema_test_service.Api.Controllers.v1;

public class BanksController(IUtilityService utilityService) : BaseControllerV1
{
    private readonly IUtilityService _utilityService = utilityService;

    /// <summary>
    /// Get banks
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("banks")]
    public async Task<IActionResult> GetBanks(CancellationToken cancellationToken = default)
    {
        BanksResponse banks = await _utilityService.GetBanksAsync(cancellationToken);
        return Ok(new BaseResponse<BanksResponse>
        {
            ResponseData = banks,
        });
    }
}
