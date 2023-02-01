using Application.Configuration;
using Application.Spotify;
using Application.CLI.Arguments;
using Application.CLI.Messages;
using Application.Handlers;

namespace Application.Dispatch;

public class Dispatch
{
  private static Dictionary<string, Func<Client, ArgumentParser, Task<int>>> _dispatchers = new Dictionary<string, Func<Client, ArgumentParser, Task<int>>>() {
    { "help", (Client _, ArgumentParser _) => {
      Initialisation.GetProgramArguments().ShowHelp();
      return Task.FromResult(0);
    }},
    { "login", (Client client, ArgumentParser _) => LoginHandler.Dispatch(client) },
    { "logout",(Client client, ArgumentParser _) => Task.FromResult(LogoutHandler.Dispatch(client)) },
    { "export", (Client client, ArgumentParser argParser) => ExportHandler.Dispatch(client, argParser) }
  };

  public static Func<Client, ArgumentParser, Task<int>> GetDispatcher(string key)
  {
    if (_dispatchers.ContainsKey(key))
    {
      return _dispatchers[key];
    }

    return (Client _, ArgumentParser _) =>
    {
      Errors.UnsupportedArgument(key);
      Initialisation.GetProgramArguments().ShowHelp();
      return Task.FromResult(1);
    };
  }
}