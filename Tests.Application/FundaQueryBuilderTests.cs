using Application.Brokers.Funda;
using Application.Brokers.Funda.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Web;

namespace Tests.Application;

public class FundaQueryBuilderTests
{
    private readonly IFundaQueryBuilder _fundaQueryBuilder;

    public FundaQueryBuilderTests()
    {
        _fundaQueryBuilder = new FundaQueryBuilder();
    }

    [Fact]
    public void Build_WithGardenAndLocation_ReturnsQuery()
    {
        // Arrange
        var options = Options.Create(new FundaSettings
        {
            ApiKey = Guid.NewGuid().ToString()
        });

        // Act
        var query = _fundaQueryBuilder.Build("/feeds/Aanbod.svc", options.Value.ApiKey, new FundaSearchOptions
        {
            CurrentPage = 3,
            Location = "rotterdam",
            PageSize = 10,
            Type = QueryType.Buy,
            WithGarden = true
        });

        // Assert
        var decoded = HttpUtility.UrlDecode(query);
        decoded.Should().Be($"/feeds/Aanbod.svc/json/{options.Value.ApiKey}/?type=koop&zo=/rotterdam/tuin/&page=3&pagesize=10");
    }

    [Fact]
    public void Build_WithoutGardenAndWithLocation_ReturnsQuery()
    {
        // Arrange
        var options = Options.Create(new FundaSettings
        {
            ApiKey = Guid.NewGuid().ToString()
        });

        // Act
        var query = _fundaQueryBuilder.Build("/feeds/Aanbod.svc", options.Value.ApiKey, new FundaSearchOptions
        {
            CurrentPage = 1,
            Location = "rotterdam",
            PageSize = 10,
            Type = QueryType.Buy,
            WithGarden = false
        });

        // Assert
        var decoded = HttpUtility.UrlDecode(query);
        decoded.Should().Be($"/feeds/Aanbod.svc/json/{options.Value.ApiKey}/?type=koop&zo=/rotterdam/&page=1&pagesize=10");
    }
}