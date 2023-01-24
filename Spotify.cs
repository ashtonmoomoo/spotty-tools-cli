namespace Spotify
{
  class Constants
  {
    public static readonly string API_BASE_URL = "https://api.spotify.com";
    public static readonly string ACCOUNTS_BASE_URL = "https://accounts.spotify.com";
  }

  interface ISpotifyClient
  {
    public void Login();
  }

  class Client : ISpotifyClient
  {
    private readonly HttpClient httpClient = new HttpClient();

    private string _clientId = Utils.Env.RequireEnvVar("SPOTIFY_CLIENT_ID");
    private string _clientSecret = Utils.Env.RequireEnvVar("SPOTIFY_CLIENT_SECRET");
    private string _redirectUri = Utils.Env.RequireEnvVar("SPOTIFY_REDIRECT_URI");
    private string _responseType = "code";
    private string _state;
    private string _scopes = "user-read-private";
    private string _authToken = String.Empty;
    private ResponseType.AccessToken? _accessTokenResponse;

    public Client()
    {
      this._state = System.Guid.NewGuid().ToString();
      // Come back to this
      this.httpClient.BaseAddress = new Uri(Spotify.Constants.ACCOUNTS_BASE_URL);
    }

    public void Login()
    {
      Utils.Browser.Open($"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type={_responseType}&redirect_uri={_redirectUri}&state={_state}&scope={_scopes}");
      var (token, state) = GetTokenAndState();

      this._authToken = token;
      ExchangeToken();
      CommitSession();
      Success();
    }

    private HttpRequestMessage ConstructRequest()
    {
      var postBody = new Dictionary<string, string>() {
        { "grant_type", "authorization_code" },
        { "code", this._authToken },
        { "redirect_uri", this._redirectUri },
      };

      string toEncode = String.Join(":", new List<string> { this._clientId, this._clientSecret });
      var encodedSecret = Utils.Encoding.Base64Encode(toEncode);

      var request = new HttpRequestMessage(HttpMethod.Post, "/api/token");
      request.Content = new FormUrlEncodedContent(postBody);
      request.Headers.Add("Authorization", $"Basic {encodedSecret}");

      return request;
    }

    private ResponseType.AccessToken ParseTokenResponse(HttpResponseMessage response)
    {
      Stream contentStream = response.Content.ReadAsStream();
      StreamReader readStream = new StreamReader(contentStream, System.Text.Encoding.UTF8);
      string content = readStream.ReadToEnd()!;

      return System.Text.Json.JsonSerializer.Deserialize<ResponseType.AccessToken>(content)!;
    }

    private void ExchangeToken()
    {
      var request = ConstructRequest();

      HttpResponseMessage response = this.httpClient.Send(request);
      response.EnsureSuccessStatusCode();

      this._accessTokenResponse = ParseTokenResponse(response);
    }

    private void Success()
    {
      Console.WriteLine("Logged in!");
    }

    // Help persist access across sessions
    private void CommitSession()
    {
      var sessionJsonString = System.Text.Json.JsonSerializer.Serialize(this._accessTokenResponse);
      string storageDir = Utils.FileSystem.Storage.GetStorageLocation();
      Utils.FileSystem.Write.WriteToFile($"{storageDir}/.session", sessionJsonString);
    }

    private (string, string) GetTokenAndState()
    {
      Utils.Web.HttpServer server = new Utils.Web.HttpServer();
      var (token, state) = server.StartAndListenOnce();
      if (state != this._state)
      {
        throw new CustomException.InvalidOAuthStateException();
      }

      return (token, state);
    }
  }

  namespace CustomException
  {
    class InvalidOAuthStateException : Exception { };
  }

  namespace ResponseType
  {
    public class AccessToken
    {
      public string? access_token { get; set; }
      public string? token_type { get; set; }
      public int? expires_in { get; set; }
      public string? refresh_token { get; set; }
      public string? scope { get; set; }
    }
  }
}
