namespace AITravelPlanner.Application.Places;

public sealed class ListPlaces(IPlaceCatalog catalog)
{
    public async Task<PlacesPage> ExecuteAsync(PlacesQuery query, CancellationToken cancellationToken)
    {
        var snapshot = await catalog.ReadAsync(cancellationToken);
        var candidates = snapshot.Where(place => place.IsCandidate).ToArray();
        foreach (var place in candidates)
        {
            place.EnsurePublicDataIsValid();
        }
        if (candidates.Select(place => place.Id).Distinct().Count() != candidates.Length)
        {
            throw new InvalidOperationException("Duplicate catalog identity.");
        }

        var filtered = candidates.Where(place =>
            (query.Category is null || place.Category == query.Category)
            && (query.District is null || place.District == query.District)).ToArray();
        var items = filtered.OrderByDescending(place => place.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(place => new PlaceDto(place.Id, place.Name!, place.Category!, place.District!)).ToArray();
        var totalPages = (int)(((long)filtered.Length + query.PageSize - 1) / query.PageSize);
        return new PlacesPage(items, query.Page, query.PageSize, filtered.Length, totalPages);
    }
}
