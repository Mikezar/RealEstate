using Application.Brokers.Funda.Interfaces;
using Application.Common;
using Domain.Offers;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Net;

namespace Application.Brokers.Funda.Implementations;

internal sealed class FundaBrokerAdapter : IFundaBrokerAdapter
{
    private const int PageSize = 25;
    private readonly IFundaGateway _fundaGateway;
    private readonly ILogger<FundaBrokerAdapter> _logger;

    public FundaBrokerAdapter(
        IFundaGateway fundaGateway,
        ILogger<FundaBrokerAdapter> logger)
    {
        _fundaGateway = fundaGateway;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BrokerWithRealEstateCount>> GetBrokersWithRealEstateObjectCount(Query query, bool withGarden)
    {
        var brokerCounts = new Dictionary<long, BrokerWithRealEstateCount>();
        var processedCount = 0;
        var currentPage = 1;
        int? totalCount = null;

        while (totalCount is null || processedCount < totalCount.Value)
        {
            try
            {
                var result = await _fundaGateway.GetMarketObjects(new FundaSearchOptions
                {
                    Type = QueryType.Buy,
                    CurrentPage = currentPage,
                    PageSize = PageSize,
                    Location = query.QueryString,
                    WithGarden = withGarden
                });

                foreach (var obj in result.Objects)
                {
                    if (brokerCounts.ContainsKey(obj.MakelaarId))
                    {
                        brokerCounts[obj.MakelaarId].IncrementObjectCount();
                    }
                    else
                    {
                        var broker = new Broker(obj.MakelaarId, obj.MakelaarNaam);
                        brokerCounts.Add(obj.MakelaarId, new BrokerWithRealEstateCount(broker, 1));
                    }
                }

                processedCount += result.Objects.Length;
                totalCount = result.TotaalAantalObjecten;
                currentPage++;
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError(ex.Message);

                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    break;
                }
            }
        }

        return brokerCounts.Values.ToImmutableList();
    }
}
