using System;
using NUnit.Framework;

namespace Scanii.Tests
{
  [TestFixture]
  public class ScaniiClientsTests
  {
    [Test]
    public void ShouldRejectNullSecret()
    {
      Assert.Throws<ArgumentNullException>((TestDelegate)(() => ScaniiClients.CreateDefault("key", null)));
    }

    [Test]
    public void ShouldRejectNullKey()
    {
      Assert.Throws<ArgumentNullException>((TestDelegate)(() => ScaniiClients.CreateDefault(null, "secret")));
    }

    [Test]
    public void ShouldRejectColonInKey()
    {
      Assert.Throws<ArgumentException>((TestDelegate)(() => ScaniiClients.CreateDefault("foo:bar", "secret")));
    }

    [Test]
    public void ShouldRejectNullAuthToken()
    {
      Assert.Throws<ArgumentNullException>((TestDelegate)(() => ScaniiClients.CreateDefault((Models.ScaniiAuthToken)null)));
    }
  }
}
