using Application.Handlers;
using Application.Spotify;
using Application.CLI.Arguments;

namespace Application.Commands;

public class LogoutCommand : Command
{
  public override string Alias
  {
    get
    {
      return "logout";
    }
  }

  public override string Description
  {
    get
    {
      return "Delete your Spotify session.";
    }
  }

  public override Func<Client, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (Client client, ArgumentParser _) => Task.FromResult(LogoutHandler.Dispatch(client));
  }
}
