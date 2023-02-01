using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;
using Application.Handlers;
using Application.Spotify;

public class Program
{
  private static Client _client = new Client(new HttpClient());
  private static ProgramArguments _arguments = Initialisation.GetProgramArguments();

  static async Task<int> Main(string[] args)
  {
    Initialisation.StartUp(_client);

    if (args.Length == 0)
    {
      Errors.NoArguments();
      _arguments.ShowHelp();
      return 1;
    }

    await Dispatch(args);
    return 0;
  }

  private static async Task<int> Dispatch(string[] args)
  {
    ArgumentParser argParser = new ArgumentParser(args);
    string thisArg = argParser.NextArg();

    switch (thisArg)
    {
      case "help":
        {
          _arguments.ShowHelp();
          return 0;
        }
      case "login":
        {
          return await LoginHandler.Dispatch(_client);
        }
      case "logout":
        {
          return LogoutHandler.Dispatch(_client);
        }
      case "export":
        {
          return await ExportHandler.Dispatch(_client, argParser);
        }
      default:
        {
          Errors.UnsupportedArgument(thisArg);
          _arguments.ShowHelp();
          return 1;
        }
    }
  }
}
