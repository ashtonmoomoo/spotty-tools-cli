namespace Application.Common.Utilities.Encoding;

public class Base64
{
  public static string Encode(string text)
  {
    var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
    return System.Convert.ToBase64String(textBytes);
  }
}
