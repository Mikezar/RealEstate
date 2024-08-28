using Application.Brokers.Funda.Dtos;
using Application.Brokers.Funda.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Application.Brokers.Funda.Implementations;

internal sealed class FundaGateway : IFundaGateway
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<FundaSettings> _settings;
    private const string OffersRelativeUrl = "/feeds/Aanbod.svc";

    public FundaGateway(
        HttpClient httpClient, 
        IOptions<FundaSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async ValueTask<MarketOverview> GetMarketObjects(FundaSearchOptions searchOptions)
    {
        var queryParams = BuildQueryParams(searchOptions);
        var marketOffers =  await _httpClient.GetAsync($"{OffersRelativeUrl}/json/{_settings.Value.ApiKey}/{queryParams}");
        marketOffers.EnsureSuccessStatusCode();
        
        var json = await marketOffers.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MarketOverview>(json);
    }

    private static string BuildQueryParams(FundaSearchOptions searchOptions)
    {
        var type = searchOptions.Type switch
        {
            QueryType.Buy => "koop",
            _ => throw new ArgumentOutOfRangeException(nameof(searchOptions.Type))
        };

        var zo = BuildSearchLocation(searchOptions);
        var queryBuilder = new QueryBuilder
        {
            { "type", type },
            { "zo", zo },
            { "page", searchOptions.CurrentPage.ToString() },
            { "pagesize", searchOptions.PageSize.ToString() }
        };

        return queryBuilder.ToString();
    }

    private static string BuildSearchLocation(FundaSearchOptions searchOptions)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"/{searchOptions.Location}/");

        if (searchOptions.WithGarden)
        {
            stringBuilder.Append("tuin/");
        }

        return stringBuilder.ToString();
    }
}
