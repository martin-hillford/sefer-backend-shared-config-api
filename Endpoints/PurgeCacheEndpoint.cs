namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class PurgeCacheEndpoint
{
    public static IResult PurgeCache(IServiceProvider provider)
    {
        var networkProvider = provider.GetNetworkProvider();
        networkProvider.PurgeCache();
        AdminConfigEndpoint.PurgeCache();
        return Results.Ok();
    }

}