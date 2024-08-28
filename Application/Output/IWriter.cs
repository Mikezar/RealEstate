using Domain.Offers;

namespace Application.Output;

public interface IWriter
{
    Task Send(string decription, BrokerWithRealEstateCount[] brokers);
}