using Application.Common.Utilities.Env;

using System.Net;

namespace Application.Common.Utilities.Server;

class HttpServer
{
  private HttpListener _listener;
  private string _localAddress = Variables.GetEnvVarOrDefault("LOCAL_ADDRESS", "http://localhost");
  private int _port = int.Parse(Variables.GetEnvVarOrDefault("PORT", "3002"));

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
      throw new InvalidSpotifyResponseException();
    }

    return (token, state);
  }
}