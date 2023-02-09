using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class User(
  [property: JsonPropertyName("country")] string Country,
  [property: JsonPropertyName("display_name")] string DisplayName,
  [property: JsonPropertyName("id")] string Id
);
