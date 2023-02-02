using Application.Configuration;
using Application.Spotify;
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

  public override Func<Client, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (Client _, ArgumentParser _) =>
    {
      Initialisation.GetProgramArguments().ShowHelp();
      return Task.FromResult(0);
    };
  }
}
