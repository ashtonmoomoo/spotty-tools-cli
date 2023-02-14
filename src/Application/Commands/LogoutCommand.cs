using Application.Handlers;
using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class LogoutCommand : ICommand
{
  public string Alias
  {
    get => "logout";
  }

  public string Description
  {
    get => "Delete your Spotify session.";
  }

  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
    => (IClient client, ArgumentParser _) => Task.FromResult(LogoutHandler.Dispatch(client));
}
