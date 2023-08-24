using Application.Common.Utilities.FileSystem;

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

  public static async Task StartUp(Interfaces.IClient client)
  {
    CreateStorageLocationIfRequired();
    await client.PrepareSession();
  }
}
