using Application.CLI.Arguments;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;
using Application.Interfaces;

namespace Application.Handlers;

public class ExportHandler
{
  public static async Task<int> Dispatch(IClient client, ArgumentParser argParser)
  {
    if (!client.IsLoggedIn())
    {
      Application.CLI.Messages.Errors.NotLoggedIn();
      return 1;
    }

    var nextArg = argParser.NextArg();

    switch (nextArg)
    {
      case "playlist":
        {
          var playlistName = argParser.NextArg();
          await ExportPlaylist(playlistName, argParser.NextArg(), client);
          return 0;
        }
    }

    return 0;
  }

  private static async Task ExportPlaylist(string playlistName, string path, IClient client)
  {
    var playlistId = await FindPlaylistIdByName(playlistName, client);
    var tracks = await client.GetPlaylistTracks(playlistId);
    Application.Spotify.Exporters.CsvExporter.WriteTracksToCsv(tracks, path);
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
