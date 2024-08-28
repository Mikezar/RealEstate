using Application.Brokers.Funda;
using Application.Brokers.Funda.Dtos;
using Application.Brokers.Funda.Implementations;
using Application.Brokers.Funda.Interfaces;
using Application.Common;
using Domain.Offers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Application;

public class FundaBrokerAdapterTests
{
    private readonly IFundaBrokerAdapter _adapter;
    private readonly IFundaGateway _fundaGateway;

    public FundaBrokerAdapterTests()
    {
        var logger = Substitute.For<ILogger<FundaBrokerAdapter>>();
        _fundaGateway = Substitute.For<IFundaGateway>();
        _adapter = new FundaBrokerAdapter(_fundaGateway, logger);
    }

    [Fact]
    public async Task GetBrokersWithRealEstateObjectCount_ReturnsResult()
    {
        // Arrange
        var query = QueryParser.Parse("Amsterdam");

        _fundaGateway.GetMarketObjects(Arg.Is<FundaSearchOptions>(
            options => options.CurrentPage == 1 && 
            options.Location == query.QueryString && 
            options.Type == QueryType.Buy && 
            options.WithGarden == true))
            .Returns(new MarketOverview
            {
                Objects =
                [
                    new FundaObject { MakelaarId = 1, MakelaarNaam = "1" },
                    new FundaObject { MakelaarId = 1, MakelaarNaam = "1" },
                    new FundaObject { MakelaarId = 3, MakelaarNaam = "3" },
                ],
                TotaalAantalObjecten = 75
            });
        _fundaGateway.GetMarketObjects(Arg.Is<FundaSearchOptions>(
            options => options.CurrentPage == 2 &&
            options.Location == query.QueryString &&
            options.Type == QueryType.Buy &&
            options.WithGarden == true)).Returns(new MarketOverview
        {
            Objects =
            [
                new FundaObject { MakelaarId = 1, MakelaarNaam = "1" },
                new FundaObject { MakelaarId = 2, MakelaarNaam = "2" },
                new FundaObject { MakelaarId = 1, MakelaarNaam = "1" },
            ],
            TotaalAantalObjecten = 75
        });
        _fundaGateway.GetMarketObjects(Arg.Is<FundaSearchOptions>(options => options.CurrentPage == 3 &&
            options.Location == query.QueryString &&
            options.Type == QueryType.Buy &&
            options.WithGarden == true)).Returns(new MarketOverview
        {
            Objects =
            [
                new FundaObject { MakelaarId = 2, MakelaarNaam = "2" },
                new FundaObject { MakelaarId = 2, MakelaarNaam = "2" },
                new FundaObject { MakelaarId = 2, MakelaarNaam = "2" },
            ],
            TotaalAantalObjecten = 75
        });
        _fundaGateway.GetMarketObjects(Arg.Is<FundaSearchOptions>(options => options.CurrentPage == 4))
            .Returns(new MarketOverview
            {
                Objects = [],
                TotaalAantalObjecten = 75
            });

        // Act
        var brokers = await _adapter.GetBrokersWithRealEstateObjectCount(query, true);

        // Assert
        brokers.Should().HaveCount(3);
        brokers.Single(b => b.Broker.Id == 1).ObjectsCount.Should().Be(4);
        brokers.Single(b => b.Broker.Id == 2).ObjectsCount.Should().Be(4);
        brokers.Single(b => b.Broker.Id == 3).ObjectsCount.Should().Be(1);
    }
}