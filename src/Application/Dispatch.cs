using Application.Interfaces;
using Application.CLI.Arguments;
using Application.CLI.Messages;

namespace Application.Dispatch;

public class Dispatch
{
  public static Func<IClient, ArgumentParser, Task<int>> GetDispatcher(string key, Commands.ICommand[] allowedCommands)
  {
    foreach (var command in allowedCommands)
    {
      if (command.Alias == key)
      {
        return command.GetDispatcher();
      }
    }

    return (IClient _, ArgumentParser _) =>
    {
      Errors.UnsupportedArgument(key);
      return Task.FromResult(1);
    };
  }
}
