using Application.CLI.Arguments;

namespace Application.Handlers;

public class ExportHandler
{
  public static async Task<int> Dispatch(Application.Spotify.Client client, ArgumentParser argParser)
  {
    var nextArg = argParser.NextArg();

    switch (nextArg)
    {
      case "playlist":
        {
          var playlistName = argParser.NextArg();
          await client.ExportPlaylist(playlistName, argParser.NextArg());
          return 0;
        }
    }

    return 0;
  }
}
