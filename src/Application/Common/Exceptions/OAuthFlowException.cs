namespace Application.Common.Exceptions;

public class OAuthFlowException : Exception
{
  public OAuthFlowException(string message) : base(message) { }

  public OAuthFlowException() : base() { }
}
