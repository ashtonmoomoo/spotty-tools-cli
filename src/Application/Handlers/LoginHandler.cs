using Application.CLI.Messages;

namespace Application.Handlers;

public class LoginHandler
{
  public static async Task<int> Dispatch(Application.Spotify.Client client)
  {
    if (!client.IsLoggedIn())
    {
      await client.Login();
      return 0;
    }

    Warnings.AlreadyLoggedIn();
    return 1;
  }
}
