using Application.Brokers.Funda.Dtos;

namespace Application.Brokers.Funda.Interfaces;

internal interface IFundaGateway
{
    ValueTask<MarketOverview> GetMarketObjects(FundaSearchOptions searchOptions);
}