namespace Application.Brokers.Funda;

public sealed class FundaSearchOptions
{
    public QueryType Type { get; init; }

    public string Location { get; init; }

    public bool WithGarden { get; init; }

    public int CurrentPage { get; init; }

    public int PageSize { get; init; }
}
