using Application.CLI.Arguments;
using Application.CLI.Messages;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;
using Application.Interfaces;
using Application.Common.Utilities.FileSystem;

namespace Application.Handlers;

public class ExportHandler
{
  public static async Task<int> Dispatch(IClient client, ArgumentParser argParser, IFileWriter writer)
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
          await ExportPlaylist(playlistName, argParser.NextArg(), client, writer);
          return 0;
        }
    }

    Errors.UnsupportedArgument(nextArg);

    return 1;
  }

  public static Task<int> Dispatch(IClient client, ArgumentParser argParser)
    => Dispatch(client, argParser, new FileWriter());

  private static async Task ExportPlaylist(string playlistName, string path, IClient client, IFileWriter fileWriter)
  {
    var playlistId = await FindPlaylistIdByName(playlistName, client);
    var tracks = await client.GetPlaylistTracks(playlistId);
    var exporter = new Application.Spotify.Exporters.TrackToCsvExporter(fileWriter);
    exporter.Export(tracks, path);
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
