public class Argument
{
  public List<string> aliases = new List<string>();
  public string name = String.Empty;
  public string description = String.Empty;

  public Argument(List<string> aliases, string name, string description)
  {
    this.aliases = aliases;
    this.name = name;
    this.description = description;
  }
}

class Arguments
{
  public List<Argument> arguments = new List<Argument>();

  public void AddArgument(List<string> aliases, string name, string description)
  {
    arguments.Add(new Argument(aliases, name, description));
  }

  public void ShowHelp()
  {
    Console.WriteLine("spotty-tools-cli");
    Console.Write("* Command *");
    Console.Write("\t");
    Console.Write("* Name *");
    Console.Write("\t");
    Console.WriteLine("* Description *");
    foreach (Argument arg in this.arguments)
    {
      Console.Write(arg.aliases[0]);
      Console.Write("\t\t");
      Console.Write(arg.name);
      Console.Write("\t\t");
      Console.Write(arg.description);
      Console.WriteLine();
    }
  }
}
