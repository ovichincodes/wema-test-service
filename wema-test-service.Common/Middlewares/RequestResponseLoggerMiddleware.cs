namespace wema_test_service.Common.Middlewares;

public sealed class RequestResponseLoggerMiddleware(RequestDelegate next, ILogger<RequestResponseLoggerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggerMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);
        await LogResponse(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        string body = null;
        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        _logger.LogInformation($"\nIncoming Request -\n \tMethod: {context.Request.Method}\n \tPath: {context.Request.Path}\n \tBody: {body}\n \tRequest Time: {DateTime.UtcNow}\n");
    }

    private async Task LogResponse(HttpContext context)
    {
        Stream originalBodyStream = context.Response.Body;

        using MemoryStream responseBody = new();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        catch
        {
            context.Response.Body = originalBodyStream;
            throw;
        }

        try
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(context.Response.Body).ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"\nOutgoing Response - \n \tStatus Code: {context.Response.StatusCode}\n \tBody: {text}\n \tResponse Time: {DateTime.UtcNow}\n");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "REQUEST_LOGGER_MIDDLEWARE => An error occured while logging response.");
        }
        finally
        {
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
