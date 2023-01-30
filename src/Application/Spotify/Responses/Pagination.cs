using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class Pagination<T>(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("items")] T[] Items,
  [property: JsonPropertyName("limit")] int Limit,
  [property: JsonPropertyName("next")] string? Next,
  [property: JsonPropertyName("offset")] int Offset,
  [property: JsonPropertyName("previous")] string? Previous,
  [property: JsonPropertyName("total")] int Total
);
