using Application.Spotify.Responses;

namespace Tests.Factories;

public class PageFactory
{
  // Return all pages for some items
  public static List<Pagination<T>> PaginateItems<T>(List<T> items, int limit)
  {
    var result = new List<Pagination<T>>();
    var total = items.Count();

    var requiredPages = System.Math.Ceiling(1f * total / limit);
    var offset = 0;

    for (int i = 0; i < requiredPages; i++)
    {
      result.Add(
        new Pagination<T>(
          string.Empty,
          items.GetRange(offset, limit).ToArray(),
          limit,
          string.Empty,
          offset,
          string.Empty,
          total
        )
      );

      offset += limit;
    }

    return result;
  }
}
