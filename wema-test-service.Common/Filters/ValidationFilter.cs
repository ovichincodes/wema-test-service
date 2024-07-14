namespace wema_test_service.Common.Filters;

public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.HttpContext.Response.ContentType = WtsConstants.ApplicationJson;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            IEnumerable<string> errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                .SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage)
                .ToList();

            BaseResponse<object> responseObj = new()
            {
                ResponseCode = ResponseCodes.BadRequest,
                ResponseMessage = ResponseMessages.ValidationError,
                ResponseData = errors
            };

            context.Result = new JsonResult(responseObj);
        }
    }
}
