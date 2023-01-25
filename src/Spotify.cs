using Application.Common.Utilities.Env;
using Application.Common.Utilities.Web;
using Application.Common.Utilities.Encoding;
using Application.Common.Utilities.FileSystem;
using Application.Common.Utilities.Server;

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
    public bool LoadSessionIfExists();
  }

  class Client : ISpotifyClient
  {
    private readonly HttpClient httpClient = new HttpClient();

    private string _clientId = Variables.RequireEnvVar("SPOTIFY_CLIENT_ID");
    private string _clientSecret = Variables.RequireEnvVar("SPOTIFY_CLIENT_SECRET");
    private string _redirectUri = Variables.RequireEnvVar("SPOTIFY_REDIRECT_URI");
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
      Browser.Open($"https://accounts.spotify.com/authorize?client_id={_clientId}&response_type={_responseType}&redirect_uri={_redirectUri}&state={_state}&scope={_scopes}");

      GetAuthToken();
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
      var encodedSecret = Base64.Encode(toEncode);

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

      var tokenResponse = ParseTokenResponse(response);

      AdornTokenResponseWithExpiryTime(tokenResponse);

      this._accessTokenResponse = tokenResponse;
    }

    private void AdornTokenResponseWithExpiryTime(ResponseType.AccessToken tokenResponse)
    {
      var lifetime = (double)(tokenResponse.expires_in != null ? tokenResponse.expires_in : 3600);
      var expiry = DateTime.UtcNow;
      expiry.AddSeconds(lifetime);
      tokenResponse.expires_at = expiry;
    }

    private void Success()
    {
      Console.WriteLine("Logged in!");
    }

    // Help persist access across sessions
    private void CommitSession()
    {
      var sessionJsonString = System.Text.Json.JsonSerializer.Serialize(this._accessTokenResponse);
      string storageDir = Storage.GetStorageLocation();
      Write.WriteToFile($"{storageDir}/.session", sessionJsonString);
    }

    public bool LoadSessionIfExists()
    {
      string storageDir = Storage.GetStorageLocation();
      string? sessionJson = Read.ReadFile($"{storageDir}/.session");
      if (sessionJson == null)
      {
        NoSessionFound();
        return false;
      }

      ResponseType.AccessToken? existingSession = System.Text.Json.JsonSerializer.Deserialize<ResponseType.AccessToken>(sessionJson);
      if (existingSession != null)
      {
        this._accessTokenResponse = existingSession;
      }

      return true;
    }

    private HttpRequestMessage ConstructRefreshRequest()
    {
      var refreshToken = this._accessTokenResponse?.refresh_token;
      if (refreshToken == null)
      {
        throw new RefreshTokenMissingException();
      }

      var postBody = new Dictionary<string, string>() {
        { "grant_type", "refresh_token" },
        { "refresh_token", refreshToken },
      };

      string toEncode = String.Join(":", new List<string> { this._clientId, this._clientSecret });
      var encodedSecret = Base64.Encode(toEncode);

      var request = new HttpRequestMessage(HttpMethod.Post, "/api/token");
      request.Content = new FormUrlEncodedContent(postBody);
      request.Headers.Add("Authorization", $"Basic {encodedSecret}");

      return request;
    }

    public void DoTokenRefresh()
    {
      var request = ConstructRefreshRequest();

      var response = this.httpClient.Send(request);
      response.EnsureSuccessStatusCode();

      var tokenResponse = ParseTokenResponse(response);

      AdornTokenResponseWithExpiryTime(tokenResponse);

      var refreshToken = this._accessTokenResponse?.refresh_token;
      this._accessTokenResponse = tokenResponse;
      this._accessTokenResponse.refresh_token = refreshToken;

      CommitSession();
    }

    private void NoSessionFound()
    {
      Console.WriteLine("No existing spotty session found");
      Console.WriteLine("Run the `login` command to create one");
    }

    private void GetAuthToken()
    {
      HttpServer server = new HttpServer();
      var (token, state) = server.StartAndListenOnce();
      if (state != this._state)
      {
        throw new InvalidOAuthStateException();
      }

      this._authToken = token;
    }
  }

  namespace ResponseType
  {
    public class AccessToken
    {
      public string? access_token { get; set; }
      public string? token_type { get; set; }
      public int? expires_in { get; set; }
      public DateTime? expires_at { get; set; }
      public string? refresh_token { get; set; }
      public string? scope { get; set; }
    }
  }
}
