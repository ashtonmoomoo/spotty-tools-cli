using Application.Handlers;
using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class AddCommand : Command
{
  public override string Alias
  {
    get
    {
      return "add";
    }
  }

  public override string Description
  {
    get
    {
      return "Add the specified items to a delegate resource.";
    }
  }

  public override Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (IClient client, ArgumentParser argParser) => AddHandler.Dispatch(client, argParser);
  }
}
