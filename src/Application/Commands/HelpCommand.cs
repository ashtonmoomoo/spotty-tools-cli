using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class HelpCommand : Command
{
  public override string Alias
  {
    get
    {
      return "help";
    }
  }

  public override string Description
  {
    get
    {
      return "Show this message.";
    }
  }

  public override Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (IClient _, ArgumentParser _) =>
    {
      Application.CLI.Messages.Help.ShowHelp(Commands.AllowedCommands);
      return Task.FromResult(0);
    };
  }
}
