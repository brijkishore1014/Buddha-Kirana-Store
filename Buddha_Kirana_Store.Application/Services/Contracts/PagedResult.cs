namespace Buddha_Kirana_Store.Services;

/// <summary>One page of a larger result. <c>Page</c> is 1-based.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

internal static class PagingRules
{
    public const int MaxPageSize = 200;

    public static (int Page, int PageSize) Normalize(int page, int pageSize) =>
        (Math.Max(1, page), Math.Clamp(pageSize, 1, MaxPageSize));
}
