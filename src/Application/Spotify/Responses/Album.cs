using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class AlbumLite(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("artists")] Artist[] Artists,
  [property: JsonPropertyName("uri")] string URI
);
