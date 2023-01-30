using Application.CLI.Messages;

using Application.Spotify.Interfaces;
using Application.Spotify.Responses;
using Application.Spotify.Exceptions;

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
    if (IsLoggedIn())
    {
      LoadLastSession();
    }
    else
    {
      PromptUser();
      await DoOAuthHandshake();
      Info.LoginSuccess();
    }
  }

  public void LoadLastSession()
  {
    var loggedIn = LoadSessionIfExists();
    this._isLoggedIn = loggedIn;
  }

  public void Logout()
  {
    ClearSession();
  }

  public async Task ExportPlaylist(string playlistName)
  {
    var playlistId = await FindPlaylistIdByName(playlistName);
    var tracks = await GetPlaylistTracks(playlistId);
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

  public async Task<List<TrackWithAddedAt>> GetLibrary()
  {
    var options = new PageOptions();
    options.Limit = 50;
    options.Offset = 0;

    string firstPage = $"{Constants.API_BASE_URL}/me/tracks?{options.QueryString()}";

    return await HandlePagination<TrackWithAddedAt>(firstPage);
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
    var request = new HttpRequestMessage(method, link);
    request.Headers.Add("Authorization", $"Bearer {this.AccessToken}");
    var response = await Http.SendRequestAndParseAs<T>(request, this.httpClient);
    if (response == null)
    {
      throw new Exception();
    }

    return response;
  }
}
