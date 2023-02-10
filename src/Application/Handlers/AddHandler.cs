using Application.Spotify.Responses;
using Application.Spotify;
using Application.Interfaces;

namespace Application.Handlers;

public class AddHandler
{
  private static class AllowedResources
  {
    public static readonly string[] RESOURCES = { "albums" };
  }

  public static async Task<int> Dispatch(IClient client, Application.CLI.Arguments.ArgumentParser argParser)
  {
    var resource = argParser.NextArg();
    if (String.IsNullOrWhiteSpace(resource) || !AllowedResources.RESOURCES.Contains(resource))
    {
      // Show some message about allowed resources
      return 1;
    }

    var destination = argParser.NextArg();
    if (String.IsNullOrWhiteSpace(destination))
    {
      // Show some message about being a required argument
      return 1;
    }

    await AddAlbumsToPlaylist(destination, client);

    return 0;
  }

  public static async Task AddAlbumsToPlaylist(string playlistName, IClient client)
  {
    var albums = await client.GetAlbums();
    var allTrackIds = GetTrackIdsFromAlbums(albums);
    var tracksCount = allTrackIds.Count();

    var playlistIds = new List<string>();
    var numberOfPlaylists = System.Math.Ceiling(1f * tracksCount / Constants.Playlist.MAX_LENGTH);
    for (var i = 1; i <= numberOfPlaylists; i++)
    {
      var id = await client.CreatePlaylist($"{playlistName} Part {i}");
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
        await client.AddSongsToPlaylist(batch.ToList(), playlistId);
        progress++;
        Console.WriteLine($"Processed {progress} / {numberOfBatches} batches...");
      }

      playlistIndex++;
    }
  }

  private static List<string> GetTrackIdsFromAlbums(List<AlbumWithAddedAt> albums)
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
}