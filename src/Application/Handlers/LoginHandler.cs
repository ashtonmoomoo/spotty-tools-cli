using Application.CLI.Messages;

namespace Application.Handlers;

public class LoginHandler
{
  public static int Dispatch(Spotify.Client client)
  {
    if (!client.IsLoggedIn())
    {
      client.Login();
      return 0;
    }

    Warnings.AlreadyLoggedIn();
    return 1;
  }
}
