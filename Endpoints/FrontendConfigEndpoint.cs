namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class FrontendConfigEndpoint
{
    public static async Task<IResult> Get(IServiceProvider provider, HttpRequest request)
    {
        var hasHeader = request.Headers.TryGetValue("X-Origin-Host", out var hostname);
        if (!hasHeader) return Results.NotFound();
        return await Get(provider, request, hostname);
    }

    public static async Task<IResult> Get(IServiceProvider provider, HttpRequest request, string hostname)
    {
        // Get the required services
        var networkProvider = provider.GetNetworkProvider();

        // 1. Check if the hostname is know. If not or
        //.   if the site is a redirect return a 400
        var site = await networkProvider.GetSiteAsync(hostname);
        if (site == null) return Results.NotFound();

        // 2. Determine if this is a redirect
        if (site.Type == SiteType.Redirect) return Results.Json(site);

        // 3. Get the region for the site
        var region = await GetRegion(provider, site, request);

        // 4. Get the environment for the site
        var environment = await networkProvider.GetEnvironmentAsync(site.Environment);

        // 5. Return the config
        var config = new FrontendConfig(site, region, environment) { Region = region.Id };
        return Results.Json(config);
    }

    private static async Task<IRegion> GetRegion(IServiceProvider provider, ISite site, HttpRequest request)
    {
        // Get the required services and the optional query provided region
        var query = request.Query.TryGetValue("region", out var regionFromQuery);
        var networkProvider = provider.GetNetworkProvider();
        var requestService = provider.GetService<IHttpRequestService>();

        // 1. For non-dynamic sites, the region is set to site's region
        if (site.Type != SiteType.Dynamic)
        {
            var siteRegion = await networkProvider.GetRegionAsync(site.RegionId);
            if (siteRegion != null) return siteRegion;
        }

        // 2. Check if the region is sent with the query
        if (query && !string.IsNullOrEmpty(regionFromQuery.ToString()))
        {
            var queryRegion = await networkProvider.GetRegionAsync(regionFromQuery);
            if (queryRegion != null) return queryRegion;
        }

        // 3. Check if a region can be found by a geo lookup
        var geoRegionId = await requestService.GetRegionId(request);
        var geoRegion = await networkProvider.GetRegionAsync(geoRegionId);
        if (geoRegion != null) return geoRegion;

        // If everything fails send the default region
        var regions = await networkProvider.GetRegionsAsync();
        return regions.Single(s => s.IsDefault);
    }
}