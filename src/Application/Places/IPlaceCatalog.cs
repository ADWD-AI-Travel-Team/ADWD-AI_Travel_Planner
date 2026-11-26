using AITravelPlanner.Domain.Places;

namespace AITravelPlanner.Application.Places;

public interface IPlaceCatalog
{
    ValueTask<IReadOnlyList<Place>> ReadAsync(CancellationToken cancellationToken);
}
