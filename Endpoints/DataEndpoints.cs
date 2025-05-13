namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class DataEndpoint
{
    public static async Task<IResult> GetSitesAsync(INetworkProvider networkProvider)
    {
        var sites = await networkProvider.GetSitesAsync();
        var data = sites.Where(s => s.Environment == "production");
        return Results.Json(data);
    }

    public static async Task<IResult> GetRegionsAsync(INetworkProvider networkProvider)
    {
        var regions = await networkProvider.GetRegionsAsync();
        return Results.Json(regions);
    }
}