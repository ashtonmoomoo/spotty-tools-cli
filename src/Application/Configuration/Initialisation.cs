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

  public static void StartUp(Application.Spotify.Client client)
  {
    CreateStorageLocationIfRequired();
    client.LoadLastSession();
  }
}
