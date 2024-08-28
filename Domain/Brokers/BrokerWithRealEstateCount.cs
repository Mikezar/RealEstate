namespace Domain.Offers;

public sealed class BrokerWithRealEstateCount
{
    public BrokerWithRealEstateCount(Broker broker, int objectsCount)
    {
        Broker = broker;
        ObjectsCount = objectsCount;
    }

    public Broker Broker { get; }

    public int ObjectsCount { get; private set; }

    public void IncrementObjectCount()
    {
        ObjectsCount++;
    }
}

