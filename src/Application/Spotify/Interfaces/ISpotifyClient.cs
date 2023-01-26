namespace Application.Spotify.Interfaces;

public interface ISpotifyClient
{
  public void Login();
  public void Logout();
  public bool IsLoggedIn();
}
