# Watchlist Tracker

Jellyfin-Plugin für Jellyfin 10.9 / .NET 8. Das Plugin speichert gesehene Episoden in einer SQLite-Datenbank im Plugin-Datenordner (`watchlisttracker/watchlist.db`).

## Build

Voraussetzungen: .NET 8 SDK und Zugriff auf die Jellyfin-NuGet-Quelle. Im Projektordner:

```bash
dotnet restore
dotnet build -c Release
```

Für ein Jellyfin-Installationspaket werden `Jellyfin.Plugin.WatchlistTracker.dll`, `meta.json` und die Web-Ressourcen in einen Ordner gelegt und als ZIP gepackt. In Jellyfin unter Dashboard → Plugins → Repositories/Plugins installieren und Jellyfin neu starten.

## Nutzung

Nach Installation ist die Oberfläche unter `/WatchlistTracker/ui` erreichbar. Die Wiedergabe-Events markieren TV-Episoden automatisch ab dem konfigurierten Mindestprozentsatz (Standard: 90 %). Serien können gesucht, gefiltert, exportiert, manuell ergänzt oder gelöscht werden.

Die Konfiguration befindet sich in den Plugin-Einstellungen: AutoTrack, TrackMovies und MinimumPlayPercentage.
