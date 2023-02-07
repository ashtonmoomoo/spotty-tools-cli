namespace Application.Handlers;

public class AddHandler
{
  private static class AllowedResources
  {
    public static readonly string[] RESOURCES = { "albums" };
  }

  public static async Task<int> Dispatch(Application.Spotify.Client client, Application.CLI.Arguments.ArgumentParser argParser)
  {
    var resource = argParser.NextArg();
    if (String.IsNullOrWhiteSpace(resource) || !AllowedResources.RESOURCES.Contains(resource))
    {
      // Show some message about allowed resources
      return 1;
    }

    var destination = argParser.NextArg();
    if (String.IsNullOrWhiteSpace(destination))
    {
      // Show some message about being a required argument
      return 1;
    }

    // Call client here

    return 0;
  }
}