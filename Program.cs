using Arguments;

class Program
{
  static ProgramArguments GetProgramArguments()
  {
    ProgramArguments arguments = new ProgramArguments();
    arguments.AddArgument(new List<string> { "login" }, "Login", "Login with your Spotify account.");

    return arguments;
  }

  static int Main(string[] args)
  {
    ProgramArguments arguments = GetProgramArguments();

    if (args.Length == 0)
    {
      Console.WriteLine("spotty-tools requires at least one argument!");
      Console.WriteLine();
      arguments.ShowHelp();
      return 1;
    }

    ArgumentParser argParser = new ArgumentParser(args);
    string thisArg = argParser.NextArg();

    switch (thisArg)
    {
      case "login": { break; }
      default:
        {
          Console.WriteLine(String.Format("`{0}` is not a supported argument.", thisArg));
          arguments.ShowHelp();
          return 1;
        }
    }

    return 0;
  }
}
