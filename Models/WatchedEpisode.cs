namespace Jellyfin.Plugin.WatchlistTracker.Models;

public sealed class WatchedEpisode
{
    public long Id { get; set; }
    public Guid JellyfinItemId { get; set; }
    public string SeriesName { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string EpisodeName { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public DateTime WatchedAt { get; set; }
    public long PlaybackPositionTicks { get; set; }
    public long RunTimeTicks { get; set; }
    public string? UserId { get; set; }
    public string? PosterPath { get; set; }
}
