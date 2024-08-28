using Application.Brokers.Funda.Dtos;
using Application.Brokers.Funda.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimiting;
using System.Threading.RateLimiting;

namespace Application.Brokers.Funda.Implementations;

internal sealed class FundaGatewayDecorator : IFundaGateway
{
    private readonly ResiliencePipeline _pipeline;
    private readonly IFundaGateway _fundaGateway;

    public FundaGatewayDecorator(
        IFundaGateway fundaGateway, 
        ILogger<FundaGatewayDecorator> logger)
    {
        _pipeline = new ResiliencePipelineBuilder()
            .AddRateLimiter(new SlidingWindowRateLimiter(
                new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    SegmentsPerWindow = 10,
                    QueueLimit = 1000,
                    Window = TimeSpan.FromMinutes(1),
                }))
             .AddRateLimiter(new RateLimiterStrategyOptions
             {
                 OnRejected = args =>
                 {
                     logger.LogWarning("Rate limit has been exceeded");
                     return default;
                 }
             }).Build();
        _fundaGateway = fundaGateway;
    }

    public async ValueTask<MarketOverview> GetMarketObjects(FundaSearchOptions searchOptions)
    {
        return await _pipeline.ExecuteAsync((_) => _fundaGateway.GetMarketObjects(searchOptions));
    }
}