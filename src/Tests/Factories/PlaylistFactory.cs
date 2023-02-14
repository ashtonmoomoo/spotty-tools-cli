using System.Collections.Generic;

using Application.Spotify.Responses;

namespace Tests.Factories;

public class PlaylistFactory
{
  public static PlaylistLite MakePlaylistLite(string name)
  {
    var ids = IdFactory.MakeId();
    return new PlaylistLite(
      false,
      $"{name} playlist description",
      ids.Href,
      ids.Id,
      name,
      true,
      ids.URI
    );
  }
}