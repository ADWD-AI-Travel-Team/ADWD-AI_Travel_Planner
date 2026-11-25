using AITravelPlanner.Application.Places;

namespace AITravelPlanner.WebApi.Places;

public static class PlacesQueryParser
{
    public static (PlacesQuery Query, Dictionary<string, string[]> Errors) Parse(IQueryCollection parameters)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var page = 1;
        var pageSize = 10;
        string? category = null;
        string? district = null;
        foreach (var (key, values) in parameters)
        {
            string? message = null;
            var value = values.Count == 1 ? values[0] : null;
            if (key is not ("page" or "pageSize" or "category" or "district"))
                message = "Unknown query parameter.";
            else if (values.Count > 1)
                message = "Query parameter must occur once.";
            else if (string.IsNullOrEmpty(value))
                message = "Value must not be empty.";
            else if (key is "page" or "pageSize")
            {
                var maximum = key == "page" ? 100000 : 50;
                if (value.Any(character => character is < '0' or > '9'))
                    message = "Value must contain ASCII digits only.";
                else if (!TryBoundedInteger(value, maximum, out var number))
                    message = $"Value must be between 1 and {maximum}.";
                else if (key == "page") page = number;
                else pageSize = number;
            }
            else if (key == "category")
            {
                if (value is not ("cafe" or "food" or "attraction"))
                    message = "Value must be one of: cafe, food, attraction.";
                else category = value;
            }
            else
            {
                if (value is not ("q1" or "q3")) message = "Value must be one of: q1, q3.";
                else district = value;
            }
            if (message is not null) errors.Add(key, [message]);
        }
        return (new PlacesQuery(page, pageSize, category, district), errors);
    }

    private static bool TryBoundedInteger(string text, int maximum, out int number)
    {
        number = 0;
        foreach (var character in text)
        {
            var digit = character - '0';
            if (number > (maximum - digit) / 10) return false;
            number = number * 10 + digit;
            if (number > maximum) return false;
        }
        return number >= 1;
    }
}
