using Application.CLI.Messages;
using Application.CLI.Arguments;
using Application.Configuration;

public class Program
{
  private static Spotify.Client _client = new Spotify.Client();

  static ProgramArguments GetProgramArguments()
  {
    ProgramArguments arguments = new ProgramArguments();
    arguments.AddArgument(new List<string> { "login" }, "Login", "Login with your Spotify account.");

    return arguments;
  }

  static void OnStartUp()
  {
    Initialisation.StartUp();
  }

  static int Main(string[] args)
  {
    OnStartUp();

    ProgramArguments arguments = GetProgramArguments();

    if (args.Length == 0)
    {
      Errors.NoArguments();
      arguments.ShowHelp();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string thisArg = argParser.NextArg();

    switch (thisArg)
    {
      case "login":
        {
          if (!_client.IsLoggedIn())
          {
            _client.Login();
          }
          else
          {
            Warnings.AlreadyLoggedIn();
          }

          return 0;
        }
      default:
        {
          Errors.UnsupportedArgument(thisArg);
          arguments.ShowHelp();
          return 1;
        }
    }
  }
}
