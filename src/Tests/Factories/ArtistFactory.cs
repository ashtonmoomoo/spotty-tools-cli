using Application.Spotify.Responses;
using System.Collections.Generic;

namespace Tests.Factories;

public class ArtistFactory
{
  public static Artist MakeArtist(string name)
  {
    var ids = IdFactory.MakeId();
    return new Artist(ids.Href, ids.Id, name, ids.URI);
  }

  public static Artist[] MakeArtists(string name, int count)
  {
    var artists = new List<Artist>();

    for (int i = 0; i < count; i++)
    {
      artists.Add(MakeArtist($"{name} {i}"));
    }

    return artists.ToArray();
  }
}
