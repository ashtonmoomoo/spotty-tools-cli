using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;
using Application.Dispatch;
using Application.Spotify;

public class Program
{
  private static Client _client = new Client(new HttpClient());

  static async Task<int> Main(string[] args)
  {
    await Initialisation.StartUp(_client);

    if (args.Length == 0)
    {
      Errors.NoArguments();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string firstArg = argParser.NextArg();

    return await Dispatch.GetDispatcher(firstArg)(_client, argParser);
  }
}
