namespace Application.Common;

public static class QueryParser
{
    private const char Separator = ' ';

    public static Query Parse(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Query.Empty;
        }

        var normalizedQuery = query.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(query))
        {
            return Query.Empty;
        }

        return new Query(normalizedQuery);
    }
}
