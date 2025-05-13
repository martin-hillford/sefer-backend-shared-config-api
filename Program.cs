// Create the app and map the requests
var app = App.Create(args);

// Map a get that returns the config for the given host or not found when hostname
// is not found or the site is a redirect
app.MapGet("/{hostname}/config.json",
    (IServiceProvider provider, HttpRequest request, [FromRoute] string hostname)
        => FrontendConfigEndpoint.Get(provider, request, hostname)
);

// For proxy purposes, the hostname is provided through and header that is
// set by the reverse proxy within the client
app.MapGet("/config.json",
    (IServiceProvider provider, HttpRequest request)
        => FrontendConfigEndpoint.Get(provider, request)
);

// Map a get that will purge the data context cache
app.MapGet("/api/purge-cache", PurgeCacheEndpoint.PurgeCache);

// Map a get that will handle all other requests. Depending on the hostname a
// redirect to a site will be provided or a 404 will be given.
app.MapGet("{**catchall}", RedirectEndpoint.Get);

// A Simple endpoint so the endpoint may be inspected on health
app.MapGet("/api/health", () => Results.Ok());

// Map a get to retrieve all production sites
app.MapGet("/api/sites", DataEndpoint.GetSitesAsync);

// Map a get to retrieve all regions
app.MapGet("/api/regions", DataEndpoint.GetRegionsAsync);

// Map a get to retrieve some information
app.MapGet("/api/info", InfoEndpoint.Get);

app.MapGet("/admin/{hostname}/config.json",
    (INetworkProvider provider, [FromRoute] string hostname)
        => AdminConfigEndpoint.Get(provider, hostname)
);

// Run the app
app.Run();