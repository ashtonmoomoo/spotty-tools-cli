using Application.Common.Utilities.Env;
using Application.Interfaces;

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

public class FileWriter : IFileWriter
{
  public void WriteText(string path, string content)
  {
    File.WriteAllText(path, content);
  }
}
