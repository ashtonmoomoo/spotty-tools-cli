using Application.Interfaces;
using Application.CLI.Arguments;
using Application.Handlers;

namespace Application.Commands;

public class LoginCommand : ICommand
{
  public string Alias
  {
    get => "login";
  }

  public string Description
  {
    get => "Login with your Spotify account.";
  }

  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
    => (IClient c, ArgumentParser _) => LoginHandler.Dispatch(c);
}
