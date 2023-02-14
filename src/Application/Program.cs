using Application.Commands;
using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;
using Application.Dispatch;
using Application.Spotify;

public class Program
{
  static async Task<int> Main(string[] args)
  {
    var httpClient = new HttpClient();
    var spotifyClient = new SpotifyClient(new SpotifyAuth(httpClient));

    await Initialisation.StartUp(spotifyClient);

    if (args.Length == 0)
    {
      Errors.NoArguments();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string firstArg = argParser.NextArg();

    return await Dispatch.GetDispatcher(firstArg, Commands.AllowedCommands)(spotifyClient, argParser);
  }
}
