using Microsoft.AspNetCore.Http.Extensions;
using System.Text;

namespace Application.Brokers.Funda.Implementations;

internal sealed class FundaQueryBuilder : IFundaQueryBuilder
{
    public string Build(string relativeUrl, string apiKey, FundaSearchOptions searchOptions)
    {
        var type = searchOptions.Type switch
        {
            QueryType.Buy => "koop",
            _ => throw new ArgumentOutOfRangeException(nameof(searchOptions.Type))
        };

        var zo = BuildSearchLocation(searchOptions);
        var queryBuilder = new QueryBuilder
        {
            { "type", type },
            { "zo", zo },
            { "page", searchOptions.CurrentPage.ToString() },
            { "pagesize", searchOptions.PageSize.ToString() }
        };

        var queryParams = queryBuilder.ToString();

        return $"{relativeUrl}/json/{apiKey}/{queryParams}";
    }

    private static string BuildSearchLocation(FundaSearchOptions searchOptions)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"/{searchOptions.Location}/");

        if (searchOptions.WithGarden)
        {
            stringBuilder.Append("tuin/");
        }

        return stringBuilder.ToString();
    }
}