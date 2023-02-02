using Application.Configuration;
using Application.Spotify;
using Application.CLI.Arguments;
using Application.CLI.Messages;
using Application.Commands;

namespace Application.Dispatch;

public class Dispatch
{
  private static Command[] _commands =
  {
    new LoginCommand(),
    new HelpCommand(),
    new LogoutCommand(),
    new ExportCommand()
  };

  public static Func<Client, ArgumentParser, Task<int>> GetDispatcher(string key)
  {
    foreach (var command in _commands)
    {
      if (command.Alias == key)
      {
        return command.GetDispatcher();
      }
    }

    return (Client _, ArgumentParser _) =>
    {
      Errors.UnsupportedArgument(key);
      Initialisation.GetProgramArguments().ShowHelp();
      return Task.FromResult(1);
    };
  }
}