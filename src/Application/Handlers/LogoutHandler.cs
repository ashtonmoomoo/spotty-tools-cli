using Application.CLI.Messages;

namespace Application.Handlers;

public class LogoutHandler
{
  public static int Dispatch(Application.Spotify.Client client)
  {
    if (client.IsLoggedIn())
    {
      client.Logout();
      Info.LogoutSuccess();
      return 0;
    }

    Warnings.NotLoggedIn();
    return 1;
  }
}
