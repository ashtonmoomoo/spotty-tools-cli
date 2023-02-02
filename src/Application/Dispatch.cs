using Application.Spotify;
using Application.CLI.Arguments;
using Application.CLI.Messages;

namespace Application.Dispatch;

public class Dispatch
{
  public static Func<Client, ArgumentParser, Task<int>> GetDispatcher(string key)
  {
    foreach (var command in Commands.Commands.AllowedCommands)
    {
      if (command.Alias == key)
      {
        return command.GetDispatcher();
      }
    }

    return (Client _, ArgumentParser _) =>
    {
      Errors.UnsupportedArgument(key);
      return Task.FromResult(1);
    };
  }
}
