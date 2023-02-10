using Application.Interfaces;
using Application.Spotify.Responses;

namespace Application.Spotify;

public class PageOptions
{
  public int? Limit { get; set; }
  public int? Offset { get; set; }

  public string QueryString()
  {
    var qs = String.Empty;
    if (Limit != null)
    {
      qs += $"limit={Limit.ToString()}&";
    }

    if (Offset != null)
    {
      qs += $"offset={Offset.ToString()}";
    }

    return qs;
  }
}

public class Pagination
{
  public static async Task<List<T>> HandlePagination<T>(string firstPageLink, IClientAuth auth)
  {
    var results = new List<T>();
    string? next = firstPageLink;

    do
    {
      var page = await auth.AuthedRequest<Pagination<T>>(HttpMethod.Get, next);
      results.AddRange(page.Items);
      next = page.Next;
    }
    while (next != null);

    return results;
  }
}
