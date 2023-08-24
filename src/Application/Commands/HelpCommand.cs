using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class HelpCommand : ICommand
{
  public string Alias
  {
    get => "help";
  }

  public string Description
  {
    get => "Show this message.";
  }

  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (IClient _, ArgumentParser _) =>
    {
      CLI.Messages.Help.ShowHelp(Commands.AllowedCommands);
      return Task.FromResult(0);
    };
  }
}
