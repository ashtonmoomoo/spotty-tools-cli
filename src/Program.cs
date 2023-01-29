using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;
using Application.Handlers;
using Application.Spotify;

public class Program
{
  private static Client _client = new Client(new HttpClient());
  private static ProgramArguments _arguments = Initialisation.GetProgramArguments();

  static int Main(string[] args)
  {
    Initialisation.StartUp();

    if (args.Length == 0)
    {
      Errors.NoArguments();
      _arguments.ShowHelp();
      return 1;
    }

    return Dispatch(args);
  }

  private static int Dispatch(string[] args)
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
          return LoginHandler.Dispatch(_client);
        }
      case "logout":
        {
          return LogoutHandler.Dispatch(_client);
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
