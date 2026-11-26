using AITravelPlanner.Application.Places;
using AITravelPlanner.Domain.Places;

namespace AITravelPlanner.Infrastructure.Places;

// Synthetic development data. These records do not represent verified real venues.
public sealed class Pa2FixturePlaceCatalog : IPlaceCatalog
{
    public ValueTask<IReadOnlyList<Place>> ReadAsync(CancellationToken cancellationToken)
    {
        Place[] places =
        [
            Create(6, "Fixture Cafe Six", "cafe", "q1"),
            Create(2, "Fixture Food Two", "food", "q1"),
            Create(4, "Fixture Cafe Four", "cafe", "q3"),
            Create(1, "Fixture Cafe One", "cafe", "q1"),
            Create(5, "Fixture Food Five", "food", "q3"),
            Create(3, "Fixture Attraction Three", "attraction", "q1"),
            Create(7, "Fixture Unverified Seven", "cafe", "q1") with { Verified = false },
            Create(8, "Fixture Ineligible Eight", "food", "q3") with { Eligible = false },
            Create(9, "Fixture Outside City Nine", "cafe", "q1") with { City = "OTHER" },
            Create(10, "Fixture Missing Hours Ten", "cafe", "q1") with { OpeningHours = null },
            Create(11, "Fixture Missing Menu Eleven", "food", "q3") with { Menu = null }
        ];
        return ValueTask.FromResult<IReadOnlyList<Place>>(places);
    }

    private static Place Create(int id, string name, string category, string district)
        => new(id, name, category, district, "HCMC", true, true, false, false, true,
            ["Synthetic opening-hours data"], ["Synthetic menu/ticket data"]);
}
