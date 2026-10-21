using Jellyfin.Plugin.WatchlistTracker.Models;
using Microsoft.Data.Sqlite;
using MediaBrowser.Common.Configuration;

namespace Jellyfin.Plugin.WatchlistTracker.Services;

public sealed class WatchlistService
{
    private readonly string _connectionString;
    public WatchlistService(IApplicationPaths paths)
    {
        var dir = Path.Combine(paths.DataPath, "watchlisttracker"); Directory.CreateDirectory(dir);
        _connectionString = new SqliteConnectionStringBuilder { DataSource = Path.Combine(dir, "watchlist.db") }.ToString();
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS WatchedEpisodes (Id INTEGER PRIMARY KEY AUTOINCREMENT, JellyfinItemId TEXT NOT NULL UNIQUE, SeriesName TEXT NOT NULL, SeasonNumber INTEGER NOT NULL, EpisodeNumber INTEGER NOT NULL, EpisodeName TEXT NOT NULL, Overview TEXT, WatchedAt TEXT NOT NULL, PlaybackPositionTicks INTEGER NOT NULL, RunTimeTicks INTEGER NOT NULL, UserId TEXT, PosterPath TEXT); CREATE INDEX IF NOT EXISTS IX_WatchedEpisodes_Series ON WatchedEpisodes(SeriesName);"; cmd.ExecuteNonQuery();
    }
    private SqliteConnection Open() { var c=new SqliteConnection(_connectionString); c.Open(); return c; }
    private static WatchedEpisode Map(SqliteDataReader r) => new() { Id=r.GetInt64(0), JellyfinItemId=Guid.Parse(r.GetString(1)), SeriesName=r.GetString(2), SeasonNumber=r.GetInt32(3), EpisodeNumber=r.GetInt32(4), EpisodeName=r.GetString(5), Overview=r.IsDBNull(6)?null:r.GetString(6), WatchedAt=DateTime.Parse(r.GetString(7), null, System.Globalization.DateTimeStyles.RoundtripKind), PlaybackPositionTicks=r.GetInt64(8), RunTimeTicks=r.GetInt64(9), UserId=r.IsDBNull(10)?null:r.GetString(10), PosterPath=r.IsDBNull(11)?null:r.GetString(11) };
    public async Task<WatchedEpisode?> GetBySeriesAndEpisodeAsync(string series, int season, int episode) { await using var c=Open(); await using var cmd=c.CreateCommand(); cmd.CommandText="SELECT * FROM WatchedEpisodes WHERE SeriesName=$s AND SeasonNumber=$sn AND EpisodeNumber=$en LIMIT 1"; cmd.Parameters.AddWithValue("$s",series); cmd.Parameters.AddWithValue("$sn",season); cmd.Parameters.AddWithValue("$en",episode); await using var r=await cmd.ExecuteReaderAsync(); return await r.ReadAsync()?Map(r):null; }
    public async Task<WatchedEpisode> AddOrUpdateAsync(WatchedEpisode x) { await using var c=Open(); await using var cmd=c.CreateCommand(); cmd.CommandText="INSERT INTO WatchedEpisodes (JellyfinItemId,SeriesName,SeasonNumber,EpisodeNumber,EpisodeName,Overview,WatchedAt,PlaybackPositionTicks,RunTimeTicks,UserId,PosterPath) VALUES ($id,$s,$sn,$en,$n,$o,$w,$p,$r,$u,$pp) ON CONFLICT(JellyfinItemId) DO UPDATE SET SeriesName=$s,SeasonNumber=$sn,EpisodeNumber=$en,EpisodeName=$n,Overview=$o,WatchedAt=$w,PlaybackPositionTicks=$p,RunTimeTicks=$r,UserId=$u,PosterPath=$pp; SELECT * FROM WatchedEpisodes WHERE JellyfinItemId=$id;"; Add(cmd,x); await using var r=await cmd.ExecuteReaderAsync(); await r.ReadAsync(); return Map(r); }
    private static void Add(SqliteCommand c, WatchedEpisode x) { foreach(var (k,v) in new[]{("$id",(object)x.JellyfinItemId.ToString()),("$s",x.SeriesName),("$sn",x.SeasonNumber),("$en",x.EpisodeNumber),("$n",x.EpisodeName),("$o",(object?)x.Overview??DBNull.Value),("$w",x.WatchedAt.ToUniversalTime().ToString("O")),("$p",x.PlaybackPositionTicks),("$r",x.RunTimeTicks),("$u",(object?)x.UserId??DBNull.Value),("$pp",(object?)x.PosterPath??DBNull.Value)}) c.Parameters.AddWithValue(k,v); }
    public async Task<List<WatchedEpisode>> GetAllAsync() { var a=new List<WatchedEpisode>(); await using var c=Open(); await using var q=c.CreateCommand(); q.CommandText="SELECT * FROM WatchedEpisodes ORDER BY WatchedAt DESC"; await using var r=await q.ExecuteReaderAsync(); while(await r.ReadAsync()) a.Add(Map(r)); return a; }
    public async Task<List<string>> GetAllSeriesAsync() { var a=new List<string>(); await using var c=Open(); await using var q=c.CreateCommand(); q.CommandText="SELECT DISTINCT SeriesName FROM WatchedEpisodes ORDER BY SeriesName"; await using var r=await q.ExecuteReaderAsync(); while(await r.ReadAsync()) a.Add(r.GetString(0)); return a; }
    public async Task<object> GetStatisticsAsync() { var all=await GetAllAsync(); return new { totalEpisodes=all.Count, totalSeries=all.Select(x=>x.SeriesName).Distinct(StringComparer.OrdinalIgnoreCase).Count(), lastWatched=all.FirstOrDefault()?.WatchedAt }; }
    public async Task DeleteAsync(long id) { await using var c=Open(); await using var q=c.CreateCommand(); q.CommandText="DELETE FROM WatchedEpisodes WHERE Id=$id"; q.Parameters.AddWithValue("$id",id); await q.ExecuteNonQueryAsync(); }
}
