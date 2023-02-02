using Application.Common.Exceptions;

using Application.Common.Utilities.Encoding;
using Application.Common.Utilities.FileSystem;
using Application.Common.Utilities.Server;
using Application.Common.Utilities.Env;
using Application.Common.Utilities.Web;

using Application.Spotify.Responses;

namespace Application.Spotify;
public abstract class ClientAuth
{
  protected bool _isLoggedIn = false;
  protected readonly HttpClient httpClient;

  private readonly string _clientId = Variables.RequireEnvVar("SPOTIFY_CLIENT_ID");
  private readonly string _clientSecret = Variables.RequireEnvVar("SPOTIFY_CLIENT_SECRET");
  private readonly string _redirectUri = Variables.RequireEnvVar("SPOTIFY_REDIRECT_URI");
  private readonly string _responseType = "code";
  private readonly string _scopes = "user-library-read+playlist-read-private";
  private string _state;
  private string _authToken = String.Empty;
  private AccessToken? _accessTokenResponse;
  private bool _refreshRequired = false;

  protected string? AccessToken
  {
    get
    {
      return this._accessTokenResponse?.Token;
    }
  }

  protected ClientAuth(HttpClient client)
  {
    this.httpClient = client;

    this._state = System.Guid.NewGuid().ToString();
  }

  protected void PromptUser()
  {
    Browser.Open($"{Constants.ACCOUNTS_BASE_URL}/authorize?client_id={_clientId}&response_type={_responseType}&redirect_uri={_redirectUri}&state={_state}&scope={_scopes}");
  }

  protected async Task DoOAuthHandshake()
  {
    GetAuthToken();
    await GetToken("access");
  }

  protected void ClearSession()
  {
    string storageDir = Storage.GetStorageLocation();
    string filePath = $"{storageDir}/.session";
    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }
    _isLoggedIn = false;
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

  public async Task PrepareSession()
  {
    LoadSessionIfExists();

    if (this._refreshRequired)
    {
      await GetToken("refresh");
    }
  }

  private async Task GetToken(string tokenType)
  {
    HttpRequestMessage? request = tokenType == "access"
      ? ConstructAccessRequest()
      : tokenType == "refresh"
      ? ConstructRefreshRequest()
      : null;

    if (request == null)
    {
      throw new OAuthFlowException("Invalid token request");
    }

    string toEncode = String.Join(":", new List<string> { this._clientId, this._clientSecret });
    var encodedSecret = Base64.Encode(toEncode);
    request.Headers.Add("Authorization", $"Basic {encodedSecret}");

    var tokenResponse = await Http.SendRequestAndParseAs<AccessToken>(request, this.httpClient);
    if (tokenResponse == null)
    {
      throw new OAuthFlowException("Failed to parse token response");
    }

    AdornAccessTokenAndSet(tokenResponse);
    CommitSession();
  }

  private void AdornAccessTokenAndSet(AccessToken tokenResponse)
  {
    var expiresAt = GetExpiryTime(tokenResponse);
    this._accessTokenResponse = tokenResponse with
    {
      RefreshToken = tokenResponse.RefreshToken ?? this._accessTokenResponse?.RefreshToken ?? String.Empty,
      ExpiresAt = expiresAt
    };
    this._isLoggedIn = true;
    this._refreshRequired = false;
  }

  private DateTime GetExpiryTime(AccessToken tokenResponse)
  {
    var now = DateTime.Now;
    var expiresAt = now.AddSeconds(tokenResponse.ExpiresIn);
    return expiresAt;
  }

  private void CommitSession()
  {
    var sessionJsonString = System.Text.Json.JsonSerializer.Serialize(this._accessTokenResponse);
    string storageDir = Storage.GetStorageLocation();
    Write.WriteToFile($"{storageDir}/.session", sessionJsonString);
  }

  protected void LoadSessionIfExists()
  {
    string storageDir = Storage.GetStorageLocation();
    string? sessionJson = Read.ReadFile($"{storageDir}/.session");
    if (String.IsNullOrWhiteSpace(sessionJson))
    {
      this._isLoggedIn = false;
      return;
    }

    AccessToken? existingSession = System.Text.Json.JsonSerializer.Deserialize<AccessToken>(sessionJson);
    if (existingSession == null)
    {
      this._isLoggedIn = false;
      return;
    }

    if (DateTime.Now > existingSession.ExpiresAt)
    {
      this._isLoggedIn = false;
      this._refreshRequired = true;
      this._accessTokenResponse = existingSession;
      return;
    }

    this._accessTokenResponse = existingSession;
    this._isLoggedIn = true;
    this._refreshRequired = true;
    return;
  }

  private HttpRequestMessage ConstructAccessRequest()
  {
    var postBody = new Dictionary<string, string>() {
        { "grant_type", "authorization_code" },
        { "code", this._authToken },
        { "redirect_uri", this._redirectUri },
      };

    var request = new HttpRequestMessage(HttpMethod.Post, $"{Constants.ACCOUNTS_BASE_URL}/api/token");
    request.Content = new FormUrlEncodedContent(postBody);

    return request;
  }

  private HttpRequestMessage ConstructRefreshRequest()
  {
    var refreshToken = this._accessTokenResponse?.RefreshToken;

    if (String.IsNullOrWhiteSpace(refreshToken))
    {
      throw new TokenRefreshException("Couldn't load refresh token");
    }

    var postBody = new Dictionary<string, string>() {
        { "grant_type", "refresh_token" },
        { "refresh_token", refreshToken },
      };

    var request = new HttpRequestMessage(HttpMethod.Post, $"{Constants.ACCOUNTS_BASE_URL}/api/token");
    request.Content = new FormUrlEncodedContent(postBody);

    return request;
  }
}
