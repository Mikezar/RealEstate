using Domain.Offers;
using System.Globalization;

namespace Application.Output;

internal sealed class ConsoleWriter : IWriter
{
    public async Task Send(string decription, BrokerWithRealEstateCount[] brokers)
    {
        Console.WriteLine(string.Format("{0}:", decription));
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} | {1} | {2} | {3}",
                "#".PadRight(8),
                "Broker id".PadRight(16),
                "Broker name".PadRight(60),
                "Object count".PadRight(8)));

        for (int i = 0; i < brokers.Length; i++)
        {
            int ranking = i + 1;
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} | {1} | {2} | {3}",
                ranking.ToString().PadRight(8),
                brokers[i].Broker.Id.ToString().PadRight(16),
                brokers[i].Broker.Name.ToString().PadRight(60),
                brokers[i].ObjectsCount.ToString().PadRight(8)));
        }
        Console.WriteLine();
    }
}
