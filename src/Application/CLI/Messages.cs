using Application.CLI.Arguments;

namespace Application.CLI.Messages;

public static class Help
{
  public static void Header()
  {
    Console.WriteLine();
    Console.WriteLine("spotty-tools-cli");
    Console.WriteLine();
    Console.Write("* Command *");
    Console.Write("\t");
    Console.Write("* Name *");
    Console.Write("\t");
    Console.WriteLine("* Description *");
  }

  public static void FormatArgument(Argument arg)
  {
    Console.Write(arg.aliases[0]);
    Console.Write("\t\t");
    Console.Write(arg.name);
    Console.Write("\t\t");
    Console.Write(arg.description);
    Console.WriteLine();
  }
}

public static class Info
{
  public static void WaitingToCompleteLogin()
  {
    Console.WriteLine("Waiting for login to complete...");
  }

  public static void LoginSuccess()
  {
    Console.WriteLine("Logged in!");
  }
}

public static class Warnings
{
  public static void AlreadyLoggedIn()
  {
    Console.WriteLine("Already logged in!");
  }
}

public static class Errors
{
  public static void NoArguments()
  {
    Console.WriteLine("spotty-tools requires at least one argument!");
    Console.WriteLine();
  }

  public static void UnsupportedArgument(string arg)
  {
    Console.WriteLine($"`{arg}` is not a supported argument.");
  }
}
