using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AITravelPlanner.WebApi.Tests;

public sealed class WeatherForecastSmokeTests
{
    [Fact]
    public async Task GetWeatherForecastReturnsFiveForecasts()
    {
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        using var response = await client.GetAsync("/weatherforecast");

        response.EnsureSuccessStatusCode();
        var forecasts = await response.Content.ReadFromJsonAsync<Forecast[]>();
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
        Assert.All(forecasts, forecast => Assert.False(string.IsNullOrWhiteSpace(forecast.Summary)));
    }

    private sealed record Forecast(DateOnly Date, int TemperatureC, string? Summary);
}
