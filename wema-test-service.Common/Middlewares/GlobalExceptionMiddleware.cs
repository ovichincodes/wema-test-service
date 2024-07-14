namespace wema_test_service.Common.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment env)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, env);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex, IWebHostEnvironment env)
    {
        _logger.LogError($"Error Processing Request\nMessage: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nTrace: {ex.StackTrace}");

        httpContext.Response.ContentType = WtsConstants.ApplicationJson;
        httpContext.Response.StatusCode = (int)GetStatusCode(ex);

        string errorMessage = ex.InnerException?.Message ?? ex.Message;

        BaseResponse<object> response = new();

        if (ex is BaseException)
        {
            BaseException baseException = ex as BaseException;
            (string code, string message) = GetResponseCode(baseException?.StatusCode ?? HttpStatusCode.InternalServerError);

            response.ResponseCode = code;
            response.ResponseMessage = message;
            response.ResponseData = baseException?.Message ?? ResponseMessages.InternalServer;

            if (baseException?.IsValidationProblems ?? false)
            {
                string[] validationFailures = errorMessage.Split('|');
                response.ResponseMessage = ResponseMessages.ValidationError;
                response.ResponseData = validationFailures;
            }
        }
        else
        {
            (string code, string message) = GetResponseCode(HttpStatusCode.InternalServerError);

            response.ResponseCode = code;
            response.ResponseMessage = message;
            response.ResponseData = errorMessage;
        }

        JsonSerializerSettings options = new() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response, options));
    }

    private static HttpStatusCode GetStatusCode(Exception ex)
    {
        if (ex is not BaseException internalException)
        {
            return HttpStatusCode.InternalServerError;
        }
        return internalException.StatusCode;
    }
    private static (string code, string message) GetResponseCode(HttpStatusCode statusCode)
    {
        (string code, string message) = (int)statusCode switch
        {
            400 => (ResponseCodes.BadRequest, ResponseMessages.BadRequest),
            500 => (ResponseCodes.InternalServer, ResponseMessages.InternalServer),
            _ => (ResponseCodes.InternalServer, ResponseMessages.InternalServer),
        };
        return (code, message);
    }
}
