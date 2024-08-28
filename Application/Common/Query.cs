namespace Application.Common;

public sealed class Query
{
    public static Query Empty = new Query(string.Empty);

    public Query(string queryString)
    {
        QueryString = queryString;
    }

    public string QueryString { get; }
}
