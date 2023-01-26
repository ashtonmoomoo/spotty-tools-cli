using Application.CLI.Messages;

using Application.Spotify.Interfaces;

namespace Application.Spotify;

public class Client : ClientBase, ISpotifyClient
{
  public Client() : base()
  {
    PrepareSession();
  }

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
}
