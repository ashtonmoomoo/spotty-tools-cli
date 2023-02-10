namespace Application.Interfaces;

public interface IClientAuth
{
  public bool IsLoggedIn();
  public Task Login();
  public void Logout();
  public Task PrepareSession();
  public Task<T> AuthedRequest<T>(HttpMethod method, string url);
  public Task<T> AuthedRequest<T>(HttpMethod method, string url, string? body);
}
