using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AITravelPlanner.ContractTests;

public sealed class CatalogConfigurationHttpTests
{
    [Theory]
    [InlineData("Production", true, 0)]
    [InlineData("Testing", true, 0)]
    [InlineData("Development", false, 0)]
    [InlineData("Development", true, 6)]
    public async Task Synthetic_adapter_requires_explicit_development_opt_in(string environment, bool enabled, int total)
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.UseSetting("Catalog:UsePa2Fixture", enabled.ToString());
        });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        using var response = await client.GetAsync("/places");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Headers.CacheControl?.NoStore);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var body = document.RootElement;
        Assert.Equal(total, body.GetProperty("totalItems").GetInt32());
        Assert.Equal(total == 0 ? 0 : 1, body.GetProperty("totalPages").GetInt32());
        Assert.Equal(1, body.GetProperty("page").GetInt32());
        Assert.Equal(10, body.GetProperty("pageSize").GetInt32());
        Assert.Equal(Enumerable.Range(1, total), body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("id").GetInt32()));
    }
}
