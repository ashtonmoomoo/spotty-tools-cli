namespace Application.Common.Exceptions;

public class MissingEnvVarException : Exception
{
  public MissingEnvVarException(string message) : base(message) { }
}
