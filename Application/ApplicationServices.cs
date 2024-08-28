using Application.Brokers;
using Application.Brokers.Funda;
using Application.Brokers.Funda.Implementations;
using Application.Brokers.Funda.Interfaces;
using Application.Output;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Application;

public static class ApplicationServices
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<FundaSettings>()
            .Bind(configuration.GetSection(nameof(FundaSettings)));

        services.AddHttpClient<IFundaGateway, FundaGateway>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<FundaSettings>>();
            client.BaseAddress = settings.Value.BaseAddress;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.Decorate<IFundaGateway, FundaGatewayDecorator>();
        services.AddScoped<IBrokerService, BrokerService>();
        services.AddScoped<IFundaBrokerAdapter, FundaBrokerAdapter>();
        services.AddScoped<IWriter, ConsoleWriter>();

        return services;
    }
}
