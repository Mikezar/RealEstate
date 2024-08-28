using Application.Brokers.Funda.Interfaces;
using Application.Common;
using Domain.Offers;

namespace Application.Brokers;

internal sealed class BrokerService : IBrokerService
{
    private readonly IFundaBrokerAdapter _brokerAdapter;

    public BrokerService(IFundaBrokerAdapter brokerAdapter)
    {
        _brokerAdapter = brokerAdapter;
    }

    public async Task<BrokerWithRealEstateCount[]> GetTopBrokers(Query query, bool withGarden, int top = 10)
    {
        var brokersWithCounts = await _brokerAdapter.GetBrokersWithRealEstateObjectCount(query, withGarden);
        return brokersWithCounts
            .OrderByDescending(b => b.ObjectsCount)
            .ThenBy(b => b.Broker.Name)
            .Take(top)
            .ToArray();
    }
}
