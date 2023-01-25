using Arguments;

class Initialisation
{
  public static void CreateStorageLocationIfRequired()
  {
    string location = Utils.FileSystem.Storage.GetStorageLocation();
    if (!Directory.Exists(location))
    {
      Directory.CreateDirectory(location);
    }
  }
}

class Program
{
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
          Spotify.Client client = new Spotify.Client();
          client.Login();
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
