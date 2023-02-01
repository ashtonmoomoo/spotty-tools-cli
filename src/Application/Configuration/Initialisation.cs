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
    arguments.AddArgument(new List<string> { "help" }, "Show this message.");
    arguments.AddArgument(new List<string> { "login" }, "Login with your Spotify account.");
    arguments.AddArgument(new List<string> { "logout" }, "Delete your Spotify session.");
    arguments.AddArgument(new List<string> { "export" }, "Export the specified resource.");

    return arguments;
  }

  public static void StartUp(Application.Spotify.Client client)
  {
    CreateStorageLocationIfRequired();
    client.LoadLastSession();
  }
}
