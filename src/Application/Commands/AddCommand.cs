using Application.Handlers;
using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class AddCommand : ICommand
{
  public string Alias
  {
    get => "add";
  }

  public string Description
  {
    get => "Add the specified items to a delegate resource.";
  }

  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
    => (IClient client, ArgumentParser argParser) => AddHandler.Dispatch(client, argParser);
}
