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

        while (totalCount is null || processedCount < totalCount.Value) // We need to run at least once to fetch totalCount
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

                if (result.Objects.Length == 0) // No more objects to process, helps to break the cycle if for some reason we didn't yet process all objects, but received empty array
                {
                    break;
                }
   
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
                totalCount = result.TotaalAantalObjecten; // May be changed dynamically (for example, new objects were added to the market)
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
