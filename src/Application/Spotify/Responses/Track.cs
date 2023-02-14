using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class Track(
  [property: JsonPropertyName("album")] AlbumLite Album,
  [property: JsonPropertyName("artists")] Artist[] Artists,
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("uri")] string URI
);

public record class TrackWithAddedAt(
  [property: JsonPropertyName("added_at")] DateTime AddedAt,
  [property: JsonPropertyName("track")] Track Track
);
