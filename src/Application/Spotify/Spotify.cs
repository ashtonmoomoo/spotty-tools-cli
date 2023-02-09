using Application.CLI.Messages;

using Application.Spotify.Interfaces;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;
using Application.Spotify.Exporters;

using Application.Common.Utilities.Web;

namespace Application.Spotify;

public class Client : ClientAuth, ISpotifyClient
{
  public Client(HttpClient httpClient) : base(httpClient) { }

  public bool IsLoggedIn()
  {
    return _isLoggedIn;
  }

  public async Task Login()
  {
    if (!IsLoggedIn())
    {
      PromptUser();
      await DoOAuthHandshake();
      Info.LoginSuccess();
    }
  }

  public void Logout()
  {
    ClearSession();
  }

  public async Task ExportPlaylist(string playlistName, string path)
  {
    var playlistId = await FindPlaylistIdByName(playlistName);
    var tracks = await GetPlaylistTracks(playlistId);
    CsvExporter.WriteTracksToCsv(tracks, path);
  }

  private async Task<string> FindPlaylistIdByName(string playlistName)
  {
    var playlists = await GetPlaylists();
    string? thePlayListId = null;
    foreach (PlaylistLite p in playlists)
    {
      if (p.Name == playlistName)
      {
        thePlayListId = p.Id;
        break;
      }
    }

    if (thePlayListId == null)
    {
      throw new PlaylistNotFoundException($"Playlist with name `{playlistName}` not found");
    }

    return thePlayListId;
  }

  public async Task<List<TrackWithAddedAt>> GetPlaylistTracks(string playlistId)
  {
    string link = $"{Constants.API_BASE_URL}/playlists/{playlistId}";

    var result = new List<TrackWithAddedAt>();

    var playlist = await AuthedRequest<Playlist>(HttpMethod.Get, link);
    var tracksPage = playlist.Tracks;

    result.AddRange(tracksPage.Items);
    if (tracksPage.Next == null)
    {
      return result;
    }

    var restOfTracks = await HandlePagination<TrackWithAddedAt>(tracksPage.Next);
    result.AddRange(restOfTracks);

    return result;
  }

  public async Task<List<PlaylistLite>> GetPlaylists()
  {
    var options = new PageOptions();
    options.Limit = 50;
    options.Offset = 0;

    string firstPage = $"{Constants.API_BASE_URL}/me/playlists?{options.QueryString()}";

    return await HandlePagination<PlaylistLite>(firstPage);
  }

  public async Task AddAlbumsToPlaylist(string playlistName)
  {
    var albums = await GetAlbums();
    var allTrackIds = GetTrackIdsFromAlbums(albums);
    var tracksCount = allTrackIds.Count();

    var playlistIds = new List<string>();
    var numberOfPlaylists = System.Math.Ceiling(1f * tracksCount / Constants.Playlist.MAX_LENGTH);
    for (var i = 1; i <= numberOfPlaylists; i++)
    {
      var id = await CreatePlaylist($"{playlistName} Part {i}");
      playlistIds.Add(id);
    }

    if (tracksCount > Constants.Playlist.MAX_LENGTH)
    {
      Console.WriteLine($"{Constants.Playlist.MAX_LENGTH} songs is the max playlist length. Splitting into multiple playlists...");
    }

    var superBatches = allTrackIds.Chunk(Constants.Playlist.MAX_LENGTH).Select(c => c.Chunk(Constants.Playlist.MAX_SONGS_TO_ADD));
    var playlistIndex = 0;
    foreach (var superBatch in superBatches)
    {
      var playlistId = playlistIds[playlistIndex];
      var numberOfBatches = superBatch.Count();

      var progress = 0;
      foreach (var batch in superBatch)
      {
        await AddSongsToPlaylist(batch.ToList(), playlistId);
        progress++;
        Console.WriteLine($"Processed {progress} / {numberOfBatches} batches...");
      }

      playlistIndex++;
    }
  }

  private async Task AddSongsToPlaylist(List<string> songIdsToAdd, string playlistId)
  {
    var url = $"{Constants.API_BASE_URL}/playlists/{playlistId}/tracks?uris={String.Join(",", songIdsToAdd)}";
    await AuthedRequest<PlaylistSnapshot>(HttpMethod.Post, url);
  }

  private List<string> GetTrackIdsFromAlbums(List<AlbumWithAddedAt> albums)
  {
    var allTrackIds = new List<string>();

    // Assume that album has less than 50 tracks and 
    // doesn't actually require pagination
    foreach (var album in albums)
    {
      allTrackIds.AddRange(album.Album.TracksPage.Items.Select(item => item.URI));
    }

    return allTrackIds;
  }

  public async Task<List<AlbumWithAddedAt>> GetAlbums()
  {
    var options = new PageOptions();
    options.Limit = 50;
    options.Offset = 0;

    string firstPage = $"{Constants.API_BASE_URL}/me/albums?{options.QueryString()}";

    return await HandlePagination<AlbumWithAddedAt>(firstPage);
  }

  public async Task<List<TrackWithAddedAt>> GetLibrary()
  {
    var options = new PageOptions();
    options.Limit = 50;
    options.Offset = 0;

    string firstPage = $"{Constants.API_BASE_URL}/me/tracks?{options.QueryString()}";

    return await HandlePagination<TrackWithAddedAt>(firstPage);
  }

  public async Task<User> GetCurrentUser()
  {
    string url = $"{Constants.API_BASE_URL}/me";
    return await AuthedRequest<User>(HttpMethod.Get, url);
  }

  public async Task<string> CreatePlaylist(string playlistName)
  {
    var currentUserId = (await GetCurrentUser()).Id;
    string url = $"{Constants.API_BASE_URL}/users/{currentUserId}/playlists";
    string body = $"{{\"name\":\"{playlistName}\"}}";

    return (await AuthedRequest<PlaylistLite>(HttpMethod.Post, url, body)).Id;
  }

  private async Task<List<T>> HandlePagination<T>(string firstPageLink)
  {
    var results = new List<T>();
    string? next = firstPageLink;

    do
    {
      var page = await AuthedRequest<Pagination<T>>(HttpMethod.Get, next);
      results.AddRange(page.Items);
      next = page.Next;
    }
    while (next != null);

    return results;
  }

  private async Task<T> AuthedRequest<T>(HttpMethod method, string link)
  {
    return await AuthedRequest<T>(method, link, null);
  }

  private async Task<T> AuthedRequest<T>(HttpMethod method, string link, string? body)
  {
    var request = new HttpRequestMessage(method, link);
    if (body != null)
    {
      request.Content = new StringContent(body);
    }

    request.Headers.Add("Authorization", $"Bearer {this.AccessToken}");
    var response = await Http.SendRequestAndParseAs<T>(request, this.httpClient);
    if (response == null)
    {
      throw new Exception();
    }

    return response;
  }
}
