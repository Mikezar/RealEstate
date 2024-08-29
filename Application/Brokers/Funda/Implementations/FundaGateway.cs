using Application.Brokers.Funda.Dtos;
using Application.Brokers.Funda.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Brokers.Funda.Implementations;

internal sealed class FundaGateway : IFundaGateway
{
    private readonly HttpClient _httpClient;
    private readonly IFundaQueryBuilder _fundaQueryBuilder;
    private readonly IOptions<FundaSettings> _settings;
    private const string OffersRelativeUrl = "/feeds/Aanbod.svc";

    public FundaGateway(
        HttpClient httpClient,
        IFundaQueryBuilder fundaQueryBuilder,
        IOptions<FundaSettings> settings)
    {
        _httpClient = httpClient;
        _fundaQueryBuilder = fundaQueryBuilder;
        _settings = settings;
    }

    public async ValueTask<MarketOverview> GetMarketObjects(FundaSearchOptions searchOptions)
    {
        var relativeUrl = _fundaQueryBuilder.Build(OffersRelativeUrl, _settings.Value.ApiKey, searchOptions);
        var marketOffers =  await _httpClient.GetAsync(relativeUrl);
        marketOffers.EnsureSuccessStatusCode();
        
        var json = await marketOffers.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MarketOverview>(json);
    }
}
