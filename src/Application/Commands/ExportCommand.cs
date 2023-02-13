using Application.Handlers;
using Application.Interfaces;
using Application.CLI.Arguments;

namespace Application.Commands;

public class ExportCommand : ICommand
{
  public string Alias
  {
    get => "export";
  }

  public string Description
  {
    get => "Export the specified resource.";
  }

  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (IClient client, ArgumentParser argParser) => ExportHandler.Dispatch(client, argParser);
  }
}
