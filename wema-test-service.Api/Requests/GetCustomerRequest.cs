namespace wema_test_service.Api.Requests;

public record GetCustomerRequest
{
    public string SearchText { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}
