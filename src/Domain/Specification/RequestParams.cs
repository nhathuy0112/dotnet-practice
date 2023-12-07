namespace Domain.Specification;

public class RequestParams
{
    private const int MaxPageSize = 50;
    private const int MinPageIndex = 1;
    private int _pageIndex = 1;
    private int _pageSize = 5;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = (value < MinPageIndex) ? MinPageIndex : value;
    }
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public string? Sort { get; set; }
}