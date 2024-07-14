namespace wema_test_service.Common.Models;

public sealed record BaseResponse
{
    public string ResponseCode { get; set; } = ResponseCodes.Ok;
    public string ResponseMessage { get; set; } = ResponseMessages.Ok;
}

public sealed record BaseResponse<T>
{
    public string ResponseCode { get; set; } = ResponseCodes.Ok;
    public string ResponseMessage { get; set; } = ResponseMessages.Ok;
    public T ResponseData { get; set; }
}
