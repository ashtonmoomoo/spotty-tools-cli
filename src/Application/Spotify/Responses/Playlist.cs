using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class PlaylistLite(
  [property: JsonPropertyName("collaborative")] bool Collaborative,
  [property: JsonPropertyName("description")] string Description,
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("public")] bool Public,
  [property: JsonPropertyName("tracks")] TracksLite Tracks,
  [property: JsonPropertyName("uri")] string Uri
);

public record class Playlist(
  [property: JsonPropertyName("collaborative")] bool Collaborative,
  [property: JsonPropertyName("description")] string Description,
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("public")] bool Public,
  [property: JsonPropertyName("tracks")] Pagination<TrackWithAddedAt> Tracks,
  [property: JsonPropertyName("uri")] string Uri
);
