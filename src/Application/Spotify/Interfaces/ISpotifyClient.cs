namespace Application.Spotify.Interfaces;

public interface ISpotifyClient
{
  public Task Login();
  public void Logout();
  public bool IsLoggedIn();
}
