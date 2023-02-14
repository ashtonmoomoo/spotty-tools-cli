namespace Tests.Factories;

public class IdFields
{
  public string Id { get; }
  public string Href { get; }
  public string URI { get; }

  public IdFields(string id, string href, string uri)
  {
    this.Id = id;
    this.Href = href;
    this.URI = uri;
  }
}

public class IdFactory
{
  // Stub implementation in case it needs to be changed later
  public static IdFields MakeId()
  {
    var id = System.Guid.NewGuid().ToString();

    return new IdFields(id, id, id);
  }
}
