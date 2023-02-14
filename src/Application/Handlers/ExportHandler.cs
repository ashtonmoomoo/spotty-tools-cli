using Application.CLI.Arguments;
using Application.CLI.Messages;
using Application.Handlers.SubHandlers;
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

    var subcommand = argParser.NextArg();

    switch (subcommand)
    {
      case "playlist":
        {
          var playlistName = argParser.NextArg();
          var path = argParser.NextArg();
          return await ExportPlaylistHandler.Dispatch(playlistName, path, client, writer);
        }
    }

    Errors.UnsupportedArgument(subcommand);

    return 1;
  }

  public static Task<int> Dispatch(IClient client, ArgumentParser argParser)
    => Dispatch(client, argParser, new FileWriter());
}
