namespace wema_test_service.Common.Models;

public sealed record PaginatedData<T> where T : class
{
    public PaginatedData(IEnumerable<T> records, long totalRecordsCount, int page = 1, int pageSize = 10)
    {
        Records = records;
        CurrentPage = page;
        CurrentRecordCount = Records.Count();
        TotalRecordCount = totalRecordsCount;
        TotalPages = (int)Math.Round((decimal)(totalRecordsCount / pageSize), 0, MidpointRounding.ToPositiveInfinity) + 1;
    }

    public IEnumerable<T> Records { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentRecordCount { get; set; }
    public long TotalRecordCount { get; set; }
    public int TotalPages { get; set; }
}
