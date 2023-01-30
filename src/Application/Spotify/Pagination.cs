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
