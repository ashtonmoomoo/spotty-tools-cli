using System.Text.Json.Serialization;

namespace Application.Spotify.Responses;

public record class AlbumLite(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("artists")] Artist[] Artists,
  [property: JsonPropertyName("uri")] string URI
);

public record class AlbumTrack(
  [property: JsonPropertyName("artists")] Artist[] Artists,
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("uri")] string URI
);

public record class Album(
  [property: JsonPropertyName("href")] string Href,
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("artists")] Artist[] Artists,
  [property: JsonPropertyName("uri")] string URI,
  [property: JsonPropertyName("tracks")] Pagination<AlbumTrack> TracksPage
);

public record class AlbumWithAddedAt(
  [property: JsonPropertyName("added_at")] DateTime AddedAt,
  [property: JsonPropertyName("album")] Album Album
);
