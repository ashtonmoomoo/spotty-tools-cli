using Application.CLI.Messages;

namespace Application.Handlers;

public class LogoutHandler
{
  public static int Dispatch(Application.Spotify.Client client)
  {
    if (!client.IsLoggedIn())
    {
      Errors.NotLoggedIn();
      return 1;
    }

    client.Logout();
    Info.LogoutSuccess();
    return 0;
  }
}
