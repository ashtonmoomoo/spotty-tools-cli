using Application.Spotify.Responses;

namespace Application.Spotify.Exporters;

public class CsvExporter
{
  private static string GetTrackRow(Track track)
  {
    var sanitised = new List<string>();

    new List<string>() {
      track.URI,
      track.Name,
      track.Album.Name,
      track.Artists[0].Name,
    }.ForEach(delegate (string field) { sanitised.Add($"\"{field.Replace("\"", "\"\"")}\""); });

    return String.Join(",", sanitised);
  }

  public static void WriteTracksToCsv(List<Track> tracks, string path)
  {
    var csv = new System.Text.StringBuilder();

    csv.AppendLine("uri,name,album,artist");

    foreach (var track in tracks)
    {
      csv.AppendLine(GetTrackRow(track));
    }

    File.WriteAllText(path, csv.ToString());
  }
}
