using Application.Common;
using Domain.Offers;

namespace Application.Brokers;

public interface IBrokerService
{
    Task<BrokerWithRealEstateCount[]> GetTopBrokers(Query query, bool withGarden, int top = 10);
}