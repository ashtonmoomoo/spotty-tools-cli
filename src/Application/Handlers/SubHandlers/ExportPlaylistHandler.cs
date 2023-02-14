using Application.Interfaces;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;

namespace Application.Handlers.SubHandlers;

public class ExportPlaylistHandler
{
  public static async Task<int> Dispatch(string playlistName, string path, IClient client, IFileWriter fileWriter)
  {
    var playlistId = await FindPlaylistIdByName(playlistName, client);
    var tracks = await client.GetPlaylistTracks(playlistId);
    var exporter = new Application.Spotify.Exporters.TrackToCsvExporter(fileWriter);
    exporter.Export(tracks, path);

    return 0;
  }

  private static async Task<string> FindPlaylistIdByName(string playlistName, IClient client)
  {
    var playlists = await client.GetPlaylists();
    string? thePlayListId = null;
    foreach (PlaylistLite p in playlists)
    {
      if (p.Name == playlistName)
      {
        thePlayListId = p.Id;
        break;
      }
    }

    if (thePlayListId == null)
    {
      throw new PlaylistNotFoundException($"Playlist with name `{playlistName}` not found");
    }

    return thePlayListId;
  }
}
