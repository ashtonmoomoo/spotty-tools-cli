using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class AccessToken(
  [property: JsonPropertyName("access_token")] string Token,
  [property: JsonPropertyName("token_type")] string Type,
  [property: JsonPropertyName("expires_in")] int ExpiresIn,
  [property: JsonPropertyName("refresh_token")] string RefreshToken,
  [property: JsonPropertyName("scope")] string Scope
);
