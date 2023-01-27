namespace Application.CLI.Arguments;

public class Argument
{
  public List<string> aliases = new List<string>();
  public string description = String.Empty;

  public Argument(List<string> aliases, string description)
  {
    this.aliases = aliases;
    this.description = description;
  }
}

public class ProgramArguments
{
  public List<Argument> arguments = new List<Argument>();

  public void AddArgument(List<string> aliases, string description)
  {
    arguments.Add(new Argument(aliases, description));
  }

  public void ShowHelp()
  {
    Messages.Help.Header();
    foreach (Argument arg in this.arguments)
    {
      Messages.Help.FormatArgument(arg);
    }
  }
}

public class ArgumentParser
{
  private List<string> args;

  public ArgumentParser(string[] args)
  {
    this.args = args.ToList();
  }

  public string NextArg()
  {
    // Args should have at least length 1
    if (this.args.Count() == 0)
    {
      throw new ArgumentsLengthException();
    }

    string arg = this.args[0];
    this.args.RemoveAt(0);

    return arg;
  }

  public List<string> GetArgs()
  {
    return this.args;
  }
}
