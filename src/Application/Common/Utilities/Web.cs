using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Application.Common.Utilities.Web;

class Browser
{
  public static void Open(string url)
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      Process.Start("open", url);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      Process.Start("xdg-open", url);
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
    }
    else
    {
      throw new UnsupportedPlatformException();
    }
  }
}
