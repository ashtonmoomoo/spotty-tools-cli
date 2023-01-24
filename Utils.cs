namespace Utils
{
  using System.Runtime.InteropServices;
  using System.Diagnostics;

  namespace Web
  {
    using System;
    using System.Net;
    class HttpServer
    {
      private HttpListener _listener;
      private string _localAddress = Utils.Env.GetEnvVarOrDefault("LOCAL_ADDRESS", "http://localhost");
      private int _port = int.Parse(Utils.Env.GetEnvVarOrDefault("PORT", "3002"));

      public HttpServer()
      {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"{_localAddress}:{_port}/");
      }

      public string StartAndListenOnce()
      {
        _listener.Start();

        Console.WriteLine("Waiting for request...");
        HttpListenerContext context = _listener.GetContext();
        HttpListenerRequest request = context.Request;
        string? token = request.QueryString.Get("code");
        _listener.Stop();

        if (String.IsNullOrEmpty(token))
        {
          throw new Exception("Token missing");
        }

        return token;
      }
    }
  }

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

      return envVar ?? defaultVar;
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