namespace Sefer.Backend.SharedConfig.Api.Services;

public class HttpRequestService(IServiceProvider provider) : IHttpRequestService
{
    private readonly IGeoIPService _geoService = provider.GetService<IGeoIPService>();

    private readonly INetworkProvider _networkProvider = provider.GetNetworkProvider();

    public async Task<string> GetRegionId(HttpRequest request, string regionId = null)
    {
        var region = await GetRegion(request, regionId);
        return region?.Id;
    }

    public async Task<IRegion> GetRegion(HttpRequest request, string regionId = null)
    {
        // 1) check if a specific region is set in the query string. (This is primary used for testing purposes.)
        // 2) check if a cookie is present that tell which region to use.
        // 3) check based on the geolocation to present which region
        // 4) return the default region
        return
            await GetRegionFromUrlParam(regionId?.Trim()) ??
            await GetRegionFromHeader(request) ??
            await GetRegionFromGeoLocation(request) ??
            await GetDefaultRegion();
    }

    private async Task<IRegion> GetRegionFromUrlParam(string regionId)
    {
        if (regionId?.StartsWith('.') == true) regionId = regionId[1..];
        if (string.IsNullOrEmpty(regionId)) return null;
        var regions = await _networkProvider.GetRegionsAsync();
        return regions.SingleOrDefault(s => s.Id == regionId);
    }

    private async Task<IRegion> GetRegionFromHeader(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-RegionId", out var regionId)) return null;
        var regions = await _networkProvider.GetRegionsAsync();
        return regions.SingleOrDefault(s => s.Id == regionId);
    }

    private async Task<IRegion> GetRegionFromGeoLocation(HttpRequest request)
    {
        var ipAddress = request?.GetClientIpAddress();
        if (string.IsNullOrEmpty(ipAddress)) return null;
        var geoInfo = await _geoService.GetInfo(ipAddress);
        if (geoInfo == null) return null;
        var regions = await _networkProvider.GetRegionsAsync();
        return regions.FirstOrDefault(s => s.CountryCode == geoInfo.CountryCode);
    }

    private async Task<IRegion> GetDefaultRegion()
    {
        var regions = await _networkProvider.GetRegionsAsync();
        return regions.SingleOrDefault(s => s.IsDefault);
    }
}