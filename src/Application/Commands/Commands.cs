namespace Application.Commands;

public class Commands
{
  public static readonly Interfaces.ICommand[] AllowedCommands =
  {
    new LoginCommand(),
    new HelpCommand(),
    new LogoutCommand(),
    new ExportCommand(),
    new AddCommand(),
  };
}
