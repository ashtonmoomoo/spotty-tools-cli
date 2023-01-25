using System.Runtime.InteropServices;

using Application.Common.Utilities.Env;

namespace Application.Common.Utilities.FileSystem;

class Storage
{
  public static string GetStorageLocation()
  {
    string homeDir = Variables.RequireEnvVar("HOME");
    return Variables.GetEnvVarOrDefault("STORAGE_LOCATION", $"{homeDir}/.spotty");
  }
}

class Read
{
  public static string? ReadFile(string path)
  {
    try
    {
      return File.ReadAllText(path);
    }
    catch (FileNotFoundException)
    {
      return null;
    }
  }
}

class Write
{
  public static void WriteToFile(string path, string content)
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      throw new UnsupportedPlatformException();
    }

    File.WriteAllText(path, content);
  }
}