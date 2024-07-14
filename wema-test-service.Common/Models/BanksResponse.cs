namespace wema_test_service.Common.Models;

public record BanksResponse
{
    public IEnumerable<BanksResultData> Result { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorMessages { get; set; }
    public bool HasError { get; set; }
    public DateTimeOffset TimeGenerated { get; set; }
}

public record BanksResultData
{
    public string BankName { get; set; }
    public string BankCode { get; set; }
}
