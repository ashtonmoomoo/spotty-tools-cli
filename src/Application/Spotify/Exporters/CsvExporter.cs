using Application.Spotify.Responses;
using Application.Interfaces;

namespace Application.Spotify.Exporters;

public class TrackToCsvExporter : IExporter<List<Track>>
{
  private IFileWriter _fileWriter;

  public TrackToCsvExporter(IFileWriter fileWriter)
  {
    this._fileWriter = fileWriter;
  }

  public void Export(List<Track> tracks, string path)
  {
    var csv = new System.Text.StringBuilder();

    csv.AppendLine("uri,name,album,artist");

    foreach (var track in tracks)
    {
      csv.AppendLine(Format(track));
    }

    this._fileWriter.WriteText(path, csv.ToString());
  }

  private string Format(Track track)
  {
    var sanitised = new List<string>();

    new List<string>() {
      track.URI,
      track.Name,
      track.Album.Name,
      track.Artists[0].Name,
    }.ForEach((string field) =>
      {
        sanitised.Add($"\"{field.Replace("\"", "\"\"")}\"");
      }
    );

    return String.Join(",", sanitised);
  }
}
