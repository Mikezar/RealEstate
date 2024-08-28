using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp;

internal static class Setup
{
    internal static IServiceProvider CreateServiceProvider(IConfiguration configuration)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationServices(configuration);
        serviceCollection.AddLogging(loggerBuilder =>
        {
            loggerBuilder.ClearProviders();
            loggerBuilder.AddConsole();
        });

        return serviceCollection.BuildServiceProvider();
    }

    internal static IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        return configurationBuilder.Build();
    }
}
