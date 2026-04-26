using System;

namespace Scanii
{
  public class ScaniiException : Exception
  {
    public ScaniiException(string message) : base(message)
    {
    }
  }

  public class ScaniiAuthException : ScaniiException
  {
    public ScaniiAuthException(string message) : base(message)
    {
    }
  }

  public class ScaniiRateLimitException : ScaniiException
  {
    public ScaniiRateLimitException(string message) : base(message)
    {
    }
  }
}
