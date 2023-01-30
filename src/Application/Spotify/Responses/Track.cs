using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class TracksLite(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("total")] int Total
);

public record class Track(
  [property: JsonPropertyName("album")] Album Album,
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
