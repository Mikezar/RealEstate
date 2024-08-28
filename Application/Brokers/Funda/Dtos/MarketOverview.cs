namespace Application.Brokers.Funda.Dtos;

public sealed class MarketOverview
{
    public FundaObject[] Objects { get; init; }

    public Paging Paging { get; init; }

    public int TotaalAantalObjecten { get; init; }
}
