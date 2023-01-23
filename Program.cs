class Program
{
  static Arguments GetProgramArguments()
  {
    Arguments arguments = new Arguments();
    arguments.AddArgument(new List<string> { "login" }, "Login", "Login with your Spotify account.");

    return arguments;
  }

  static int Main(string[] args)
  {
    Arguments arguments = GetProgramArguments();

    if (args.Length == 0)
    {
      Console.WriteLine("spotty-tools requires at least one argument!");
      Console.WriteLine();
      arguments.ShowHelp();
      return 1;
    }

    return 0;
  }
}
