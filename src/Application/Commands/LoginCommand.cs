using Application.Spotify;
using Application.CLI.Arguments;
using Application.Handlers;

namespace Application.Commands;

public class LoginCommand : Command
{
  public override string Alias
  {
    get
    {
      return "login";
    }
  }

  public override string Description
  {
    get
    {
      return "Login with your Spotify account.";
    }
  }

  public override Func<Client, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (Client c, ArgumentParser _) => LoginHandler.Dispatch(c);
  }
}
