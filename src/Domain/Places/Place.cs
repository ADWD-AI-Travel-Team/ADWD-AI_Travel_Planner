using System.Globalization;
using System.Text;

namespace AITravelPlanner.Domain.Places;

public sealed record Place(
    int Id, string? Name, string? Category, string? District,
    string City, bool Verified, bool Published, bool Private, bool Draft,
    bool? Eligible, IReadOnlyList<string>? OpeningHours, IReadOnlyList<string>? Menu)
{
    public bool IsCandidate => City == "HCMC" && Verified && Published && !Private && !Draft
        && Eligible != false && OpeningHours is { Count: > 0 } && Menu is { Count: > 0 };

    public void EnsurePublicDataIsValid()
    {
        if (Id < 1 || string.IsNullOrEmpty(Name) || Name.EnumerateRunes().Count() > 200
            || char.IsWhiteSpace(Name[0]) || char.IsWhiteSpace(Name[^1])
            || Name.EnumerateRunes().Any(rune => Rune.GetUnicodeCategory(rune) == UnicodeCategory.Control)
            || Category is not ("cafe" or "food" or "attraction") || District is not ("q1" or "q3"))
        {
            throw new InvalidOperationException("Invalid public catalog record.");
        }
    }
}
