using Application.Interfaces;
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

  public override Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (IClient c, ArgumentParser _) => LoginHandler.Dispatch(c);
  }
}
