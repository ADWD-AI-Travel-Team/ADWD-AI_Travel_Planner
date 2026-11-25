using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Xunit.Abstractions;

namespace AITravelPlanner.ContractTests;

public sealed class QueryParserCharacterizationTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("?page=1&page=2", "page", "1|2")]
    [InlineData("?Page=1&page=2", "page", "1|2")]
    [InlineData("?page=1&Page=2", "Page", "1|2")]
    [InlineData("?%70age=%31", "page", "1")]
    [InlineData("?page=+1", "page", " 1")]
    [InlineData("?page=%2B1", "page", "+1")]
    [InlineData("?page", "page", "")]
    [InlineData("?&&", null, null)]
    [InlineData("?category=%2563afe", "category", "%63afe")]
    [InlineData("?category=%GG", "category", "%GG")]
    [InlineData("?category=%", "category", "%")]
    [InlineData("?=x", "", "x")]
    [InlineData("?category=cafe;district=q1", "category", "cafe;district=q1")]
    public async Task TestServer_exposes_framework_parsed_query(string query, string? key, string? values)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        await using var app = builder.Build();
        app.MapGet("/", (HttpRequest request) =>
            request.Query.Select(pair => new { pair.Key, Values = pair.Value.ToArray() }));
        await app.StartAsync();
        using var client = app.GetTestClient();
        using var response = await client.GetAsync("/" + query);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        output.WriteLine("{0} => {1}", query, body);
        using var json = JsonDocument.Parse(body);
        if (key is null)
        {
            Assert.Equal(0, json.RootElement.GetArrayLength());
            return;
        }
        var entry = Assert.Single(json.RootElement.EnumerateArray());
        Assert.Equal(key, entry.GetProperty("key").GetString());
        Assert.Equal(values, string.Join("|", entry.GetProperty("values").EnumerateArray().Select(v => v.GetString())));
    }
}
