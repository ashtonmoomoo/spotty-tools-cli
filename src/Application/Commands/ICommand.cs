using Application.CLI.Arguments;
using Application.Interfaces;

namespace Application.Commands;

public abstract class Command
{
  public abstract string Alias { get; }
  public abstract string Description { get; }
  public abstract Func<IClient, ArgumentParser, Task<int>> GetDispatcher();
};
