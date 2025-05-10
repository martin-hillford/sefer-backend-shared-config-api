namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class RedirectEndpoint
{
    public static async Task<IResult> Get(IServiceProvider serviceProvider, HttpRequest request)
    {
        // 1. Get the hostname and requestUri that made the request
        var networkProvider = serviceProvider.GetNetworkProvider();
        var hostname = GetHostname(request);
        var requestUri = request.Path + request.QueryString;

        // 2. Check if there is redirect site for that hostname
        var site = await networkProvider.GetSiteAsync(hostname);
        if (site?.Type != SiteType.Redirect) return Results.NotFound();

        // 3. Check if the site has a region. Is that is the case it must be a
        //    set region redirect
        var region = networkProvider.GetRegionAsync(site.RegionId);

        // 4. Redirect to the site
        var destination = (region == null)
            ? site.Destination + requestUri
            : site.Destination + "/set-region/" + site.RegionId + "?rel=" + HttpUtility.UrlEncode(requestUri);
        return Results.Redirect(destination);
    }

    private static string GetHostname(HttpRequest request)
    {
        var fromQuery = request.Query["hostname"];
        if (!string.IsNullOrEmpty(fromQuery)) return fromQuery;
        return request.GetHostname();
    }
}
