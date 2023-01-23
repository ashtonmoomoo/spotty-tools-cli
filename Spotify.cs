namespace Spotify
{
  class Client
  {
    private string _clientId = Utils.Env.RequireEnvVar("SPOTIFY_CLIENT_ID");
    private string _redirectUri = Utils.Env.RequireEnvVar("SPOTIFY_REDIRECT_URI");
    private string _responseType = "code";
    private string _state = System.Guid.NewGuid().ToString();
    private string _scopes = String.Empty;
    private string _authToken = String.Empty;

    public Client()
    {
      this._scopes = GetApplicationScopes();
    }

    private string GetApplicationScopes()
    {
      return "user-read-private";
    }

    public void Login()
    {
      Utils.Browser.Open($"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type={_responseType}&redirect_uri={_redirectUri}&state={_state}&scope={_scopes}");
      this._authToken = GetToken();
      Console.WriteLine(this._authToken);
    }

    private string GetToken()
    {
      Utils.Web.HttpServer server = new Utils.Web.HttpServer();
      string token = server.StartAndListenOnce();
      return token;
    }
  }
}