using Application.Common.Exceptions;

namespace Application.CLI.Arguments;

public class ArgumentParser
{
  private List<string> args;

  public ArgumentParser(string[] args)
  {
    this.args = args.ToList();
  }

  public string NextArg()
  {
    if (this.args.Count() == 0)
    {
      throw new ArgumentsLengthException();
    }

    string arg = this.args[0];
    this.args.RemoveAt(0);

    return arg;
  }
}
