using Application.Interfaces;
using Application.Spotify.Responses;

namespace Application.Spotify;

public class SpotifyClient : IClient
{
  private IClientAuth auth;

  public SpotifyClient(IClientAuth auth)
  {
    this.auth = auth;
  }

  public bool IsLoggedIn()
  {
    return auth.IsLoggedIn();
  }

  public async Task Login()
  {
    await auth.Login();
  }

  public void Logout()
  {
    auth.Logout();
  }

  public async Task PrepareSession()
  {
    await auth.PrepareSession();
  }

  public async Task<List<Track>> GetPlaylistTracks(string playlistId)
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/playlists/{playlistId}/tracks?{options.QueryString()}";
    var result = await Pagination.HandlePagination<TrackWithAddedAt>(firstPage, auth);
    return result.Select(track => track.Track).ToList();
  }

  public async Task<List<PlaylistLite>> GetPlaylists()
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/me/playlists?{options.QueryString()}";
    return await Pagination.HandlePagination<PlaylistLite>(firstPage, auth);
  }

  public async Task AddSongsToPlaylist(List<string> songIdsToAdd, string playlistId)
  {
    var url = $"{Constants.API_BASE_URL}/playlists/{playlistId}/tracks?uris={String.Join(",", songIdsToAdd)}";
    await auth.AuthedRequest<PlaylistSnapshot>(HttpMethod.Post, url);
  }

  public async Task<List<Album>> GetAlbums()
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/me/albums?{options.QueryString()}";
    var results = await Pagination.HandlePagination<AlbumWithAddedAt>(firstPage, auth);
    return results.Select(album => album.Album).ToList();
  }

  public async Task<List<Track>> GetLibrary()
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/me/tracks?{options.QueryString()}";
    var result = await Pagination.HandlePagination<TrackWithAddedAt>(firstPage, auth);
    return result.Select(track => track.Track).ToList();
  }

  public async Task<User> GetCurrentUser()
  {
    string url = $"{Constants.API_BASE_URL}/me";
    return await auth.AuthedRequest<User>(HttpMethod.Get, url);
  }

  public async Task<string> CreatePlaylist(string playlistName)
  {
    var currentUserId = (await GetCurrentUser()).Id;
    string url = $"{Constants.API_BASE_URL}/users/{currentUserId}/playlists";
    string body = $"{{\"name\":\"{playlistName}\"}}";

    return (await auth.AuthedRequest<PlaylistLite>(HttpMethod.Post, url, body)).Id;
  }
}
