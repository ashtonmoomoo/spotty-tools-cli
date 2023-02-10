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

  public async Task<List<TrackWithAddedAt>> GetPlaylistTracks(string playlistId)
  {
    string link = $"{Constants.API_BASE_URL}/playlists/{playlistId}";

    var result = new List<TrackWithAddedAt>();

    var playlist = await auth.AuthedRequest<Playlist>(HttpMethod.Get, link);
    var tracksPage = playlist.Tracks;

    result.AddRange(tracksPage.Items);
    if (tracksPage.Next == null)
    {
      return result;
    }

    var restOfTracks = await Pagination.HandlePagination<TrackWithAddedAt>(tracksPage.Next, auth);
    result.AddRange(restOfTracks);

    return result;
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

  public async Task<List<AlbumWithAddedAt>> GetAlbums()
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/me/albums?{options.QueryString()}";
    return await Pagination.HandlePagination<AlbumWithAddedAt>(firstPage, auth);
  }

  public async Task<List<TrackWithAddedAt>> GetLibrary()
  {
    var options = new PageOptions() { Limit = 50, Offset = 0 };
    string firstPage = $"{Constants.API_BASE_URL}/me/tracks?{options.QueryString()}";
    return await Pagination.HandlePagination<TrackWithAddedAt>(firstPage, auth);
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
