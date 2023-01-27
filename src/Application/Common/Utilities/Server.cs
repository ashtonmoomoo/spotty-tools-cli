using Application.Common.Utilities.Env;
using Application.CLI.Messages;

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
    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(@"
<html>
<body>
  <div style='display: flex;'>
    <h1 style='margin: 0 auto;'>Safe to close this window now :)</h1>
  </div>
</body>
</html>");
    response.ContentLength64 = buffer.Length;
    System.IO.Stream output = response.OutputStream;
    output.Write(buffer, 0, buffer.Length);
    output.Close();
  }

  public (string, string) StartAndListenOnce()
  {
    _listener.Start();

    Info.WaitingToCompleteLogin();
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