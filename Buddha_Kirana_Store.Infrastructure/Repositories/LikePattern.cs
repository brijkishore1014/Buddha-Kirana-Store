namespace Buddha_Kirana_Store.Data.Repositories;

internal static class LikePattern
{
    /// <summary>Builds a "contains" pattern for ILIKE, escaping the characters that mean something to LIKE.</summary>
    public static string Contains(string term)
    {
        var escaped = term.Trim()
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
        return $"%{escaped}%";
    }
}
