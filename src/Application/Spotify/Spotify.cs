using Application.CLI.Messages;

using Application.Spotify.Interfaces;

namespace Application.Spotify;

public class Client : ClientAuth, ISpotifyClient
{
  public Client(HttpClient httpClient) : base(httpClient) { }

  public bool IsLoggedIn()
  {
    return _isLoggedIn;
  }

  public async Task Login()
  {
    if (IsLoggedIn())
    {
      LoadLastSession();
    }
    else
    {
      PromptUser();
      await DoOAuthHandshake();
      Info.LoginSuccess();
    }
  }

  public void LoadLastSession()
  {
    var loggedIn = LoadSessionIfExists();
    this._isLoggedIn = loggedIn;
  }

  public void Logout()
  {
    ClearSession();
  }
}
