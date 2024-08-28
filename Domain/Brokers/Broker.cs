namespace Domain.Offers;

public sealed class Broker
{
    public Broker(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public long Id { get; }

    public string Name { get; }
}

