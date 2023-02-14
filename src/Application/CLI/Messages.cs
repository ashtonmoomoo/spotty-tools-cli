namespace Application.CLI.Messages;

public static class Help
{
  private static void Header()
  {
    Console.WriteLine();
    Console.WriteLine("spotty-tools-cli");
    Console.WriteLine();
    Console.Write("* Command *");
    Console.Write("\t");
    Console.WriteLine("* Description *");
  }

  private static void FormatCommandHelp(Application.Interfaces.ICommand command)
  {
    Console.Write(command.Alias);
    Console.Write('\t');
    Console.Write('\t');
    Console.Write(command.Description);
    Console.WriteLine();
  }

  public static void ShowHelp(Application.Interfaces.ICommand[] commands)
  {
    Header();
    foreach (var command in commands)
    {
      FormatCommandHelp(command);
    }
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

  public static void LogoutSuccess()
  {
    Console.WriteLine("Logged out!");
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

  public static void NotLoggedIn()
  {
    Console.WriteLine("Not logged in!");
  }
}
