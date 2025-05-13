namespace Sefer.Backend.SharedConfig.Api.Endpoints;

public static class AdminConfigEndpoint
{
    private static readonly Dictionary<string, string> Configs = [];

    public static void PurgeCache() => Configs.Clear();

    public static async Task<IResult> Get(INetworkProvider provider, [FromRoute] string hostname)
    {
        if (Configs.TryGetValue(hostname, out string value)) return SendAsJson(value);

        var fetched = await FetchData(provider, hostname);
        if (fetched == null) return Results.NotFound();

        Configs.Add(hostname, fetched);
        return SendAsJson(fetched);
    }

    private static async Task<string> FetchData(INetworkProvider provider, string hostname)
    {
        try
        {
            var environment = await provider.GetEnvironmentNameForAdminAsync(hostname);
            var content = await provider.GetAdminConfigAsync(environment ?? "development");
            if (string.IsNullOrEmpty(content)) return null;
            return content;
        }
        catch (Exception) { return null; }
    }

    private static IResult SendAsJson(string json) => Results.Text(json, "application/json");
}