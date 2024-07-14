namespace wema_test_service.Common.Exceptions;

public class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsValidationProblems { get; set; }

    public BaseException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public BaseException(HttpStatusCode statusCode, string message, bool isValidationProblems = false) : base(message)
    {
        StatusCode = statusCode;
        IsValidationProblems = isValidationProblems;
    }
}
