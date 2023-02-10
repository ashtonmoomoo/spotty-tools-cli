using Application.CLI.Messages;
using Application.Interfaces;

namespace Application.Handlers;

public class LogoutHandler
{
  public static int Dispatch(IClient client)
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
