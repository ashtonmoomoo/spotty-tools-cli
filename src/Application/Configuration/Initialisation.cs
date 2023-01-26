using Application.Common.Utilities.FileSystem;
using Application.CLI.Arguments;

namespace Application.Configuration;

public class Initialisation
{
  private static void CreateStorageLocationIfRequired()
  {
    string location = Storage.GetStorageLocation();
    if (!Directory.Exists(location))
    {
      Directory.CreateDirectory(location);
    }
  }

  public static ProgramArguments GetProgramArguments()
  {
    ProgramArguments arguments = new ProgramArguments();
    arguments.AddArgument(new List<string> { "login" }, "Login", "Login with your Spotify account.");

    return arguments;
  }

  public static void StartUp()
  {
    CreateStorageLocationIfRequired();
  }
}
