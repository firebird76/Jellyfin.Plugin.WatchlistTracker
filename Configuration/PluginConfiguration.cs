using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.WatchlistTracker.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public bool AutoTrack { get; set; } = true;
    public bool TrackMovies { get; set; } = false;
    public int MinimumPlayPercentage { get; set; } = 90;
}
