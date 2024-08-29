namespace Application.Brokers.Funda.Implementations;

internal interface IFundaQueryBuilder
{
    string Build(string relativeUrl, string apiKey, FundaSearchOptions searchOptions);
}