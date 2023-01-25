using Arguments;
using Application.Common.Utilities.FileSystem;

class Initialisation
{
  public static void CreateStorageLocationIfRequired()
  {
    string location = Storage.GetStorageLocation();
    if (!Directory.Exists(location))
    {
      Directory.CreateDirectory(location);
    }
  }
}

class Program
{
  private static Spotify.Client _client = new Spotify.Client();
  private static bool _isLoggedIn = false;

  static ProgramArguments GetProgramArguments()
  {
    ProgramArguments arguments = new ProgramArguments();
    arguments.AddArgument(new List<string> { "login" }, "Login", "Login with your Spotify account.");

    return arguments;
  }

  // Create storage/config directory if it doesn't already exist
  static void OnStartUp()
  {
    Initialisation.CreateStorageLocationIfRequired();
    _isLoggedIn = _client.LoadSessionIfExists();
    if (_isLoggedIn)
    {
      _client.DoTokenRefresh();
    }
  }

  static int Main(string[] args)
  {
    OnStartUp();

    ProgramArguments arguments = GetProgramArguments();

    if (args.Length == 0)
    {
      Console.WriteLine("spotty-tools requires at least one argument!");
      Console.WriteLine();
      arguments.ShowHelp();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string thisArg = argParser.NextArg();

    switch (thisArg)
    {
      case "login":
        {
          if (!_isLoggedIn)
          {
            _client.Login();
          }
          else
          {
            Console.WriteLine("Already logged in!");
          }

          return 0;
        }
      default:
        {
          Console.WriteLine($"`{thisArg}` is not a supported argument.");
          arguments.ShowHelp();
          return 1;
        }
    }
  }
}
