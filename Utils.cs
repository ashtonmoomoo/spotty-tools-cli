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

      private void SendResponse(HttpListenerResponse response)
      {
        // Construct a response.
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("<HTML><BODY>" + "<H1>Safe to close this window now :)</H1>" + "</BODY></HTML>");
        // Get a response stream and write the response to it.
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
      }

      public (string, string) StartAndListenOnce()
      {
        _listener.Start();

        Console.WriteLine("Waiting for request...");
        HttpListenerContext context = _listener.GetContext();
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;
        SendResponse(response);

        _listener.Stop();

        string? token = request.QueryString.Get("code");
        string? state = request.QueryString.Get("state");
        if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(state))
        {
          throw new Exception("Token or state missing");
        }

        return (token, state);
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

  class Encoding
  {
    public static string Base64Encode(string text)
    {
      var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
      return System.Convert.ToBase64String(textBytes);
    }
  }

  namespace FileSystem
  {
    using System.Runtime.InteropServices;

    class Storage
    {
      public static string GetStorageLocation()
      {
        string homeDir = Utils.Env.RequireEnvVar("HOME");
        return Utils.Env.GetEnvVarOrDefault("STORAGE_LOCATION", $"{homeDir}/.spotty");
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
          throw new Exception("Writing on Windows isn't implemented yet");
        }

        File.WriteAllText(path, content);
      }
    }
  }
}