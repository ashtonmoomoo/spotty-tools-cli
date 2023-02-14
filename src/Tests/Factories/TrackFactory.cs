using System.Collections.Generic;

using Application.Spotify.Responses;

namespace Tests.Factories;

public class TrackFactory
{
  public static Track MakeTrack(string name)
  {
    var ids = IdFactory.MakeId();

    return new Track(
      AlbumFactory.MakeAlbumLite($"{name} album"),
      ArtistFactory.MakeArtists($"{name} artist", 1),
      ids.Href,
      ids.Id,
      name, ids.URI
    );
  }

  public static List<Track> MakeTracks(string name, int count)
  {
    var result = new List<Track>();

    for (int i = 0; i < count; i++)
    {
      result.Add(MakeTrack($"{name} {i}"));
    }

    return result;
  }
}