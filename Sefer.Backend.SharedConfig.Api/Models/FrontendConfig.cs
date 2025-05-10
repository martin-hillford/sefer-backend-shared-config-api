// Please note: this clas is just as an output model so not every property may accessed compile time
// ReSharper disable once UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Sefer.Backend.SharedConfig.Api.Models;

public sealed class FrontendConfig(ISite site, IRegion region, IEnvironment environment)
{
    // Api connections is done through a reverse proxy because of all the issues
    // with cors headers 
    public string Api => "/api";
    
    /// <summary>
    /// The region this front-end config is 
    /// </summary>
    public string Region { get; internal set; }

    /// <summary>
    /// The type of the site redirect, fixed, dynamic)
    /// redirect: this site is only a landing page with is redirect to another frontend
    /// fixed: This site can only handle one region
    /// dynamic: This site can handle multiple region
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SiteType Type => site.Type;

    /// <summary>
    /// The name of this site
    /// </summary>
    public string Name => site.Name;

    /// <summary>
    /// An alternative name for the website
    /// </summary>
    public string Alt => site.Alt;

    /// <summary>
    /// The language for this site (current only Dutch is supported)
    /// </summary>
    public string Language => site.Language;

    public string ImageSuffix => site.ImageSuffix;
    
    public string Brand => site.Brand;

    public string Logo => site.GetHeaderLogo(region);

    public string LogoLarge => site.GetLogoLarge(region);

    public string LogoSvg => site.GetLogoSvg(region);

    public ISocials Social => site.SocialMedia;

    public string Email => site.SupportEmail;

    public string Cdn => site.StaticContentUrl;

    public bool EnableRewards => region.EnableRewards;

    public bool Enabled => site.Enabled;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Mode? Mode => site.Mode;

    public string Url => site.SiteUrl;

    public string Ping => $"{Url}/index.html";

    public string EnvName => environment.EnvironmentName;
}