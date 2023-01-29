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

  public void Login()
  {
    PromptUser();
    DoOAuthHandshake();
    Info.LoginSuccess();
  }

  public void Logout()
  {
    ClearSession();
  }
}
