namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class InfoEndpoint
{
    public static IResult Get(HttpRequest request)
    {
        var headers = request.Headers;
        var query = request.Query;
        return Results.Json(new { headers, query });
    }
}