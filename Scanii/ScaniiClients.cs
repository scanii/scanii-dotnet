using System;
using System.Net.Http;
using Scanii.Internal;
using Scanii.Models;

namespace Scanii
{
  public static class ScaniiClients
  {
#pragma warning disable CS0618 // ScaniiTarget.Auto is deprecated — intentional backward-compat default
    public static IScaniiClient CreateDefault(string key, string secret,
      HttpClient client = null, ScaniiTarget target = null)
    {
      if (secret == null) throw new ArgumentNullException(nameof(secret));
      if (key == null) throw new ArgumentNullException(nameof(key));
      if (key.Contains(":")) throw new ArgumentException("API key must not contain ':'", nameof(key));

      client ??= new HttpClient();
      target ??= ScaniiTarget.Auto;
      return new DefaultScaniiClient(target, key, secret, client);
    }

    public static IScaniiClient CreateDefault(ScaniiAuthToken authToken,
      HttpClient client = null, ScaniiTarget target = null)
    {
      if (authToken == null) throw new ArgumentNullException(nameof(authToken));

      client ??= new HttpClient();
      target ??= ScaniiTarget.Auto;
      return new DefaultScaniiClient(target, authToken.ResourceId, "", client);
    }
#pragma warning restore CS0618
  }
}
