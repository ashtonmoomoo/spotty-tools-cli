using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;
using Application.Handlers;

public class Program
{
  private static Spotify.Client _client = new Spotify.Client();
  private static ProgramArguments _arguments = Initialisation.GetProgramArguments();

  static void OnStartUp()
  {
    Initialisation.StartUp();
  }

  static int Main(string[] args)
  {
    OnStartUp();

    if (args.Length == 0)
    {
      Errors.NoArguments();
      _arguments.ShowHelp();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string thisArg = argParser.NextArg();

    switch (thisArg)
    {
      case "login":
        {
          return LoginHandler.Dispatch(_client);
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
