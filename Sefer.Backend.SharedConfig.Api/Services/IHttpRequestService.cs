namespace Sefer.Backend.SharedConfig.Api.Services;

public interface IHttpRequestService
{
    public Task<IRegion> GetRegion(HttpRequest request, string regionId = null);

    public Task<string> GetRegionId(HttpRequest request, string regionId = null);
}
