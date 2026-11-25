namespace AITravelPlanner.Application.Places;

public sealed record PlacesQuery(int Page = 1, int PageSize = 10, string? Category = null, string? District = null);
public sealed record PlaceDto(int Id, string Name, string Category, string District);
public sealed record PlacesPage(IReadOnlyList<PlaceDto> Items, int Page, int PageSize, int TotalItems, int TotalPages);
