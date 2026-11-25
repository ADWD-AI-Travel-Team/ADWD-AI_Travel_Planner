using AITravelPlanner.Application.Places;

namespace AITravelPlanner.WebApi.Places;

public static class PlacesEndpoints
{
    public static void MapPlaces(this WebApplication app)
    {
        app.MapGet("/places", async (HttpContext context, ListPlaces useCase, ILoggerFactory loggerFactory) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            var (query, errors) = PlacesQueryParser.Parse(context.Request.Query);
            if (errors.Count > 0)
            {
                return Results.Json(new
                {
                    type = "urn:travel-planner:problem:invalid-query",
                    title = "Invalid query",
                    status = 400,
                    detail = "One or more query parameters are invalid.",
                    instance = "/places",
                    errors
                }, statusCode: 400, contentType: "application/problem+json");
            }
            try
            {
                var page = await useCase.ExecuteAsync(query, context.RequestAborted);
                return Results.Json(page);
            }
            catch (Exception exception)
            {
                loggerFactory.CreateLogger("Places").LogError(exception, "Could not read the public catalog.");
                return Results.Json(new
                {
                    type = "urn:travel-planner:problem:internal-error",
                    title = "Internal error",
                    status = 500,
                    detail = "An unexpected error occurred. Please try again later.",
                    instance = "/places"
                }, statusCode: 500, contentType: "application/problem+json");
            }
        }).AllowAnonymous();
    }
}
