using Jellyfin.Plugin.WatchlistTracker.Configuration;
using Jellyfin.Plugin.WatchlistTracker.Services;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.WatchlistTracker;

public sealed class Plugin : BasePlugin<PluginConfiguration>, IPlugin, IPluginServiceRegistrator
{
    public static readonly Guid PluginGuid = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
    public static Plugin Instance { get; private set; } = null!;
    public override string Name => "Watchlist Tracker";
    public override Guid Id => PluginGuid;
    public string Description => "Track watched episodes and manage a personal watchlist.";
    public Plugin(IApplicationPaths applicationPaths) : base(applicationPaths) => Instance = this;
    public void RegisterServices(IServiceCollection services) => services.AddSingleton<WatchlistService>();
}
