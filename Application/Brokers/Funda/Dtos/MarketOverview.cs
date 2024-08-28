namespace Application.Brokers.Funda.Dtos;

public sealed class MarketOverview
{
    public Object[] Objects { get; init; }

    public Paging Paging { get; init; }

    public int TotaalAantalObjecten { get; init; }
}
