using Application.Spotify.Responses;

namespace Application.Interfaces;

public interface IClient
{
  public Task Login();
  public void Logout();
  public bool IsLoggedIn();
  public Task PrepareSession();

  // Not ideal having the return types tightly coupled to Spotify
  // Possibly could inject the type somehow
  public Task<List<Track>> GetPlaylistTracks(string playlistId);
  public Task<List<PlaylistLite>> GetPlaylists();
  public Task<List<Album>> GetAlbums();
  public Task<List<Track>> GetLibrary();
  public Task<User> GetCurrentUser();
  public Task<string> CreatePlaylist(string playlistName);
  public Task AddSongsToPlaylist(List<string> songIdsToAdd, string playlistId);
}
