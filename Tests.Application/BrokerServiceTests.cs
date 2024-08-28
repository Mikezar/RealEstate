using Application.Brokers;
using Application.Brokers.Funda.Interfaces;
using Application.Common;
using Domain.Offers;
using FluentAssertions;
using NSubstitute;

namespace Tests.Application;

public class BrokerServiceTests
{
    private readonly IFundaBrokerAdapter _adapter;
    private readonly IBrokerService _service;

    public BrokerServiceTests()
    {
        _adapter = Substitute.For<IFundaBrokerAdapter>();
        _service = new BrokerService(_adapter);
    }

    [Fact]
    public async Task GetTopBrokers_WithTop10AndWithoutGarden_Returns10Brokers()
    {
        // Arrange
        BrokerWithRealEstateCount[] expectedResult =
            [
                new BrokerWithRealEstateCount(new Broker(11, "Broker 11"), 90), 
                new BrokerWithRealEstateCount(new Broker(4, "Broker 4"), 50),
                new BrokerWithRealEstateCount(new Broker(10, "Broker 10"), 40),
                new BrokerWithRealEstateCount(new Broker(5, "Broker 5"), 39),
                new BrokerWithRealEstateCount(new Broker(7, "Broker 7"), 37),
                new BrokerWithRealEstateCount(new Broker(6, "Broker 6"), 21),
                new BrokerWithRealEstateCount(new Broker(3, "Broker 3"), 18),
                new BrokerWithRealEstateCount(new Broker(8, "Broker 8"), 15),
                new BrokerWithRealEstateCount(new Broker(9, "Broker 9"), 13),
                new BrokerWithRealEstateCount(new Broker(1, "Broker 1"), 10)
            ];

        var query = QueryParser.Parse("Amsterdam");
        _adapter.GetBrokersWithRealEstateObjectCount(query, false)
            .Returns([
                new BrokerWithRealEstateCount(new Broker(1, "Broker 1"), 10),
                new BrokerWithRealEstateCount(new Broker(2, "Broker 2"), 9),
                new BrokerWithRealEstateCount(new Broker(3, "Broker 3"), 18),
                new BrokerWithRealEstateCount(new Broker(4, "Broker 4"), 50),
                new BrokerWithRealEstateCount(new Broker(5, "Broker 5"), 39),
                new BrokerWithRealEstateCount(new Broker(6, "Broker 6"), 21),
                new BrokerWithRealEstateCount(new Broker(7, "Broker 7"), 37),
                new BrokerWithRealEstateCount(new Broker(8, "Broker 8"), 15),
                new BrokerWithRealEstateCount(new Broker(9, "Broker 9"), 13),
                new BrokerWithRealEstateCount(new Broker(10, "Broker 10"), 40),
                new BrokerWithRealEstateCount(new Broker(11, "Broker 11"), 90),
                ]);

        // Act
        var brokers = await _service.GetTopBrokers(query, false, 10);

        // Assert
        brokers.Should().HaveCount(10);
        brokers.Should().BeEquivalentTo(expectedResult, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetTopBrokers_WithEmptyQuery_ReturnsEmptyResults()
    {
        // Arrange
        var query = QueryParser.Parse("");
        _adapter.GetBrokersWithRealEstateObjectCount(query, false)
            .Returns([
                new BrokerWithRealEstateCount(new Broker(1, "Broker 1"), 10),
                new BrokerWithRealEstateCount(new Broker(2, "Broker 2"), 9),
                new BrokerWithRealEstateCount(new Broker(3, "Broker 3"), 18),
                new BrokerWithRealEstateCount(new Broker(4, "Broker 4"), 50),
                new BrokerWithRealEstateCount(new Broker(5, "Broker 5"), 39),
                new BrokerWithRealEstateCount(new Broker(6, "Broker 6"), 21),
                new BrokerWithRealEstateCount(new Broker(7, "Broker 7"), 37),
                new BrokerWithRealEstateCount(new Broker(8, "Broker 8"), 15),
                new BrokerWithRealEstateCount(new Broker(9, "Broker 9"), 13),
                new BrokerWithRealEstateCount(new Broker(10, "Broker 10"), 40),
                new BrokerWithRealEstateCount(new Broker(11, "Broker 11"), 90),
                ]);

        // Act
        var brokers = await _service.GetTopBrokers(query, false, 10);

        // Assert
        brokers.Should().BeEmpty();
    }
}