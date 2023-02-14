namespace Application.Commands;

public class Commands
{
  public static readonly Application.Interfaces.ICommand[] AllowedCommands =
  {
    new LoginCommand(),
    new HelpCommand(),
    new LogoutCommand(),
    new ExportCommand(),
    new AddCommand(),
  };
}
