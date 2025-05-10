namespace Sefer.Backend.SharedConfig.Api;

public static class App
{
    public static WebApplication Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder
            .WithSharedConfig()
            .AddCustomCorsMiddleware()
            .Services
                .AddHttpClient()
                .AddGeoIPService(builder.Configuration, "GeoIP")
                .AddSingleton<IHttpRequestService, HttpRequestService>()
                .ConfigureHttpJsonOptions(options =>
                {
                    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

        var app = builder.Build();
        app.UseCustomCorsMiddleware();
        return app;
    }
}


