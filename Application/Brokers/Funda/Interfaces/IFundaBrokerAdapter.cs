using Application.Common;
using Domain.Offers;

namespace Application.Brokers.Funda.Interfaces;

internal interface IFundaBrokerAdapter
{
    Task<IReadOnlyList<BrokerWithRealEstateCount>> GetBrokersWithRealEstateObjectCount(Query query, bool withGarden);
}