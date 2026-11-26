using AITravelPlanner.Application.Places;
using AITravelPlanner.Domain.Places;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AITravelPlanner.ContractTests;

internal sealed class FakeCatalog : IPlaceCatalog
{
    public IReadOnlyList<Place> Places { get; set; } = ReferenceFixture.Create();
    public int Reads { get; private set; }
    public bool ThrowOnRead { get; init; }

    public ValueTask<IReadOnlyList<Place>> ReadAsync(CancellationToken cancellationToken)
    {
        Reads++;
        if (ThrowOnRead) throw new InvalidOperationException("SECRET_SENTINEL database connection failure");
        return ValueTask.FromResult<IReadOnlyList<Place>>(Places.ToArray());
    }
}

internal sealed class PlacesTestHost(FakeCatalog catalog) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPlaceCatalog>();
            services.AddSingleton<IPlaceCatalog>(catalog);
        });
    }

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        AllowAutoRedirect = false
    });
}

internal static class ReferenceFixture
{
    public static Place Valid(int id = 1, string name = "Fixture Cafe One", string category = "cafe", string district = "q1")
        => new(id, name, category, district, "HCMC", true, true, false, false, true,
            ["Synthetic hours"], ["Synthetic ticket/menu"]);

    public static Place[] Create() =>
    [
        Valid(6, "Fixture Cafe Six"),
        Valid(4, "Fixture Cafe Four", "cafe", "q3"),
        Valid(2, "Fixture Food Two", "food"),
        Valid(1),
        Valid(5, "Fixture Food Five", "food", "q3"),
        Valid(3, "Fixture Attraction Three", "attraction"),
        Valid(7, "Fixture Unverified Seven") with { Verified = false },
        Valid(8, "Fixture Ineligible Eight", "food", "q3") with { Eligible = false },
        Valid(9, "Fixture Outside City Nine") with { City = "OTHER" },
        Valid(10, "Fixture Missing Hours Ten") with { OpeningHours = null },
        Valid(11, "Fixture Missing Menu Eleven", "food", "q3") with { Menu = null },
        Valid(12, "Fixture Private Twelve") with { Private = true },
        Valid(13, "Fixture Draft Thirteen") with { Draft = true },
        Valid(14, "Fixture Unpublished Fourteen") with { Published = false }
    ];
}
