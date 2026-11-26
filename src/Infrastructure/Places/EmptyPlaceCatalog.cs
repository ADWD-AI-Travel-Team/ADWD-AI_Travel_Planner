using AITravelPlanner.Application.Places;
using AITravelPlanner.Domain.Places;

namespace AITravelPlanner.Infrastructure.Places;

public sealed class EmptyPlaceCatalog : IPlaceCatalog
{
    public ValueTask<IReadOnlyList<Place>> ReadAsync(CancellationToken cancellationToken)
        => ValueTask.FromResult<IReadOnlyList<Place>>(Array.Empty<Place>());
}
