using Application.CLI.Messages;
using Application.Interfaces;

namespace Application.Handlers;

public class LoginHandler
{
  public static async Task<int> Dispatch(IClient client)
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
