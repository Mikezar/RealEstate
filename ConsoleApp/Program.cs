using Application.Brokers;
using Application.Common;
using Application.Output;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        string queryString = string.Empty;

#if DEBUG
        queryString = "Amsterdam";
#else

        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a search query.");
            return;
        }
        queryString = args[0];
#endif

        var cofiguration = Setup.CreateConfiguration();
        var provider = Setup.CreateServiceProvider(cofiguration);

        using (var scope = provider.CreateScope())
        {
            var brokerService = scope.ServiceProvider.GetRequiredService<IBrokerService>();
            var writer = scope.ServiceProvider.GetRequiredService<IWriter>();
            var query = QueryParser.Parse(queryString);

            var resultsWithoutGarden = await brokerService.GetTopBrokers(query, withGarden: false);
            await writer.Send("Data without garden", resultsWithoutGarden);

            var resultsWithGarden = await brokerService.GetTopBrokers(query, withGarden: true);
            await writer.Send("Data with garden", resultsWithGarden);
        }

        Console.ReadLine();
    }
}
