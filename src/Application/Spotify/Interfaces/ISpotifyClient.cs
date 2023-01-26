namespace Application.Spotify.Interfaces;

public interface ISpotifyClient
{
  public void Login();
  public bool IsLoggedIn();
}
