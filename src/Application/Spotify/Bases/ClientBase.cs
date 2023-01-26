using Application.Common.Utilities.Encoding;
using Application.Common.Utilities.FileSystem;
using Application.Common.Utilities.Server;
using Application.Common.Utilities.Env;
using Application.Common.Utilities.Web;

using Application.Spotify.Responses;

namespace Application.Spotify;

public class ClientBase
{
  protected bool _isLoggedIn = false;

  private readonly HttpClient httpClient;
  private readonly string _clientId = Variables.RequireEnvVar("SPOTIFY_CLIENT_ID");
  private readonly string _clientSecret = Variables.RequireEnvVar("SPOTIFY_CLIENT_SECRET");
  private readonly string _redirectUri = Variables.RequireEnvVar("SPOTIFY_REDIRECT_URI");
  private readonly string _responseType = "code";
  private readonly string _scopes = "user-read-private";
  private string _state;
  private string _authToken = String.Empty;
  private AccessToken? _accessTokenResponse;

  protected ClientBase()
  {
    this.httpClient = new HttpClient();
    this.httpClient.BaseAddress = new Uri(Application.Spotify.Constants.ACCOUNTS_BASE_URL);
    this._state = System.Guid.NewGuid().ToString();

    PrepareSession();
  }

  protected void PromptUser()
  {
    Browser.Open($"{Constants.ACCOUNTS_BASE_URL}/authorize?client_id={_clientId}&response_type={_responseType}&redirect_uri={_redirectUri}&state={_state}&scope={_scopes}");
  }

  protected void DoOAuthHandshake()
  {
    GetAuthToken();
    ExchangeToken();
    CommitSession();
  }

  private void PrepareSession()
  {
    var sessionExists = LoadSessionIfExists();
    if (sessionExists)
    {
      this._isLoggedIn = true;
      DoTokenRefresh();
    }
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

  private AccessToken ParseTokenResponse(HttpResponseMessage response)
  {
    Stream contentStream = response.Content.ReadAsStream();
    StreamReader readStream = new StreamReader(contentStream, System.Text.Encoding.UTF8);
    string content = readStream.ReadToEnd()!;

    return System.Text.Json.JsonSerializer.Deserialize<AccessToken>(content)!;
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

  private void AdornTokenResponseWithExpiryTime(AccessToken tokenResponse)
  {
    var lifetime = (double)tokenResponse.expires_in;
    var expiry = DateTime.UtcNow;
    expiry.AddSeconds(lifetime);
    tokenResponse.expires_at = expiry;
  }

  private void CommitSession()
  {
    var sessionJsonString = System.Text.Json.JsonSerializer.Serialize(this._accessTokenResponse);
    string storageDir = Storage.GetStorageLocation();
    Write.WriteToFile($"{storageDir}/.session", sessionJsonString);
  }

  private bool LoadSessionIfExists()
  {
    string storageDir = Storage.GetStorageLocation();
    string? sessionJson = Read.ReadFile($"{storageDir}/.session");
    if (sessionJson == null)
    {
      return false;
    }

    AccessToken? existingSession = System.Text.Json.JsonSerializer.Deserialize<AccessToken>(sessionJson);
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

  private void DoTokenRefresh()
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
