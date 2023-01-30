namespace Application.Spotify.Exceptions;

public class PlaylistNotFoundException : Exception
{
  public PlaylistNotFoundException(string message) : base(message) { }
}
