// Create the app and map the requests
var app = App.Create(args);

// Returns the frontend config for the given hostname
// If the hostname is not is a redirect is returned to the default site
// Frontend that what to proxy the config.json can use this endpoint
app.MapGet("/{hostname}/config.json",
    (IServiceProvider provider, HttpRequest request, [FromRoute] string hostname)
        => FrontendConfigEndpoint.Get(provider, request, hostname)
);

// Returns the frontend config for the hostname is provided through a header that is
// set by the reverse proxy or within the client
app.MapGet("/config.json",
    (IServiceProvider provider, HttpRequest request)
        => FrontendConfigEndpoint.Get(provider, request)
);

// Purges the data context cache
app.MapGet("/data/purge-cache", PurgeCacheEndpoint.PurgeCache);

// Provides health information on this sever (useful for watchdogs)
app.MapGet("/health", () => Results.Ok());

// Retrieve all production sites
app.MapGet("/data/sites", DataEndpoint.GetSitesAsync);

// Retrieve all regions
app.MapGet("/data/regions", DataEndpoint.GetRegionsAsync);

// Retrieve some information
app.MapGet("/data/info", InfoEndpoint.Get);

// Map the admin config 
app.MapGet("/admin/{hostname}/config.json",
    (INetworkProvider provider, [FromRoute] string hostname)
        => AdminConfigEndpoint.Get(provider, hostname)
);

// Map a get that will handle all other requests. Depending on the hostname a
// redirect to a site will be provided or a 404 will be given.
app.MapGet("{**catchall}", RedirectEndpoint.Get);

// Run the app
app.Run();