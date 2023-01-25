namespace Application.Common.Utilities.Env;

class Variables
{
  public static string GetEnvVarOrDefault(string name, string defaultVar)
  {
    string? envVar = Environment.GetEnvironmentVariable(name);

    return envVar ?? defaultVar;
  }

  public static string RequireEnvVar(string name)
  {
    string? envVar = Environment.GetEnvironmentVariable(name);

    if (String.IsNullOrWhiteSpace(envVar))
    {
      throw new MissingEnvVarException($"Couldn't find environment variable {name}");
    }

    return envVar;
  }
}
