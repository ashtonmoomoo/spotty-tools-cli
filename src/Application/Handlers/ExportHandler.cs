using Application.CLI.Arguments;
using Application.CLI.Messages;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;
using Application.Interfaces;
using Application.Common.Utilities.FileSystem;

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

    Errors.UnsupportedArgument(nextArg);

    return 1;
  }

  private static async Task ExportPlaylist(string playlistName, string path, IClient client)
  {
    var playlistId = await FindPlaylistIdByName(playlistName, client);
    var tracks = await client.GetPlaylistTracks(playlistId);
    var exporter = new Application.Spotify.Exporters.TrackToCsvExporter(new FileWriter());
    exporter.Export(tracks.Select(t => t.Track).ToList(), path);
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
