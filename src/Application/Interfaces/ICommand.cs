using Application.CLI.Arguments;

namespace Application.Interfaces;

public interface ICommand
{
  public string Alias { get; }
  public string Description { get; }
  public Func<IClient, ArgumentParser, Task<int>> GetDispatcher();
};
