using Application.CLI.Arguments;
using Application.Spotify;

namespace Application.Commands;

public abstract class Command
{
  public abstract string Alias { get; }
  public abstract string Description { get; }

  public abstract Func<Client, ArgumentParser, Task<int>> GetDispatcher();
};
