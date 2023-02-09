namespace Application.Spotify;

public class Constants
{
  public static readonly string API_BASE_URL = "https://api.spotify.com/v1";
  public static readonly string ACCOUNTS_BASE_URL = "https://accounts.spotify.com";

  public static class Playlist
  {
    public static readonly int MAX_LENGTH = 11_000;
    public static readonly int MAX_SONGS_TO_ADD = 100;
  }
}
