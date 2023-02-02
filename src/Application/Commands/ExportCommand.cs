using Application.Handlers;
using Application.Spotify;
using Application.CLI.Arguments;

namespace Application.Commands;

public class ExportCommand : Command
{
  public override string Alias
  {
    get
    {
      return "export";
    }
  }

  public override string Description
  {
    get
    {
      return "Export the specified resource.";
    }
  }

  public override Func<Client, ArgumentParser, Task<int>> GetDispatcher()
  {
    return (Client client, ArgumentParser argParser) => ExportHandler.Dispatch(client, argParser);
  }
}
