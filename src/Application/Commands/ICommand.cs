using Application.CLI.Arguments;
using Application.Interfaces;

namespace Application.Commands;

public interface ICommand
{
  public string Alias { get; }
  public string Description { get; }
  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher();
};
