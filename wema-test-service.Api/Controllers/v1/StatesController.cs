namespace wema_test_service.Api.Controllers.v1;

public class StatesController(IUtilityService utilityService) : BaseControllerV1
{
    private readonly IUtilityService _utilityService = utilityService;

    /// <summary>
    /// Get states
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("states")]
    public async Task<IActionResult> GetStates(CancellationToken cancellationToken = default)
    {
        IEnumerable<string> states = await _utilityService.GetStatesAsync(cancellationToken);
        return Ok(new BaseResponse<IEnumerable<string>>
        {
            ResponseData = states,
        });
    }

    /// <summary>
    /// Get state LGAs
    /// </summary>
    /// <param name="state"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("states/{state}/lgas")]
    public async Task<IActionResult> GetLgas([FromRoute] string state, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> lgas = await _utilityService.GetStateLgasAsync(state, cancellationToken);
        return Ok(new BaseResponse<IEnumerable<string>>
        {
            ResponseData = lgas,
        });
    }
}
