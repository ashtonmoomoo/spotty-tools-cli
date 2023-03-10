using Application.Spotify.Responses;

namespace Tests.Factories;

public class AlbumFactory
{
  public static Album MakeAlbum(string name, int numberOfSongs)
  {
    var albumLite = MakeAlbumLite(name);
    var albumTracks = new List<AlbumTrack>(AlbumTrackFactory.MakeAlbumTracks($"{name} album track", numberOfSongs));
    var albumTrackPage = PageFactory.PaginateItems<AlbumTrack>(albumTracks, 50)[0];
    return new Album(
      albumLite.Href,
      albumLite.Id,
      name,
      albumLite.Artists,
      albumLite.URI,
      albumTrackPage
    );
  }

  public static Album MakeAlbum(string name)
  {
    return MakeAlbum(name, 10);
  }

  public static List<Album> MakeAlbums(string name, int numberOfAlbums)
  {
    return MakeAlbums(name, numberOfAlbums, 10);
  }

  public static List<Album> MakeAlbums(string name, int numberOfAlbums, int numberOfSongsPerAlbum)
  {
    var result = new List<Album>();

    for (int i = 0; i < numberOfAlbums; i++)
    {
      result.Add(MakeAlbum($"{name} {i}", numberOfSongsPerAlbum));
    }

    return result;
  }

  public static AlbumLite MakeAlbumLite(string name)
  {
    var ids = IdFactory.MakeId();
    var artists = ArtistFactory.MakeArtists($"{name} artist", 1);
    return new AlbumLite(ids.Href, ids.Id, name, artists, ids.URI);
  }
}

public class AlbumTrackFactory
{
  public static AlbumTrack MakeAlbumTrack(string name)
  {
    var ids = IdFactory.MakeId();
    var artists = ArtistFactory.MakeArtists($"{name} artist", 1);
    return new AlbumTrack(artists, ids.Href, ids.Id, name, ids.URI);
  }

  public static AlbumTrack[] MakeAlbumTracks(string name, int count)
  {
    var result = new List<AlbumTrack>();

    for (int i = 0; i < count; i++)
    {
      result.Add(MakeAlbumTrack($"{name} {i}"));
    }

    return result.ToArray();
  }
}