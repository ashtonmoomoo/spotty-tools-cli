using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class Artist(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("uri")] string URI
);
