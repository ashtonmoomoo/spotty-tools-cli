namespace Utils
{
  using System.Runtime.InteropServices;
  using System.Diagnostics;

  class Browser
  {
    class UnsupportedBrowserException : Exception { };

    public static void Open(string url)
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
        Process.Start("open", url);
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        Process.Start("xdg-open", url);
      }
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
      }
      else
      {
        throw new UnsupportedBrowserException();
      }
    }

  }

  class Env
  {
    public static string GetEnvVarOrDefault(string name, string defaultVar)
    {
      string? envVar = Environment.GetEnvironmentVariable(name);

      if (envVar == null)
      {
        return defaultVar;
      }

      return name;
    }

    class MissingEnvVarException : Exception
    {
      public MissingEnvVarException(string message) : base(message)
      {
      }
    };

    public static string RequireEnvVar(string name)
    {
      string? envVar = Environment.GetEnvironmentVariable(name);

      if (String.IsNullOrWhiteSpace(envVar))
      {
        throw new MissingEnvVarException($"Couldn't find environment variable {name}");
      }

      return envVar;
    }
  }
}