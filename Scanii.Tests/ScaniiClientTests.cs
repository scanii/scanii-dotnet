using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace Scanii.Tests
{
  /// <summary>
  /// Integration tests against a locally-running scanii-cli mock server.
  ///
  /// Start the server before running:
  ///   docker run -d --name scanii-cli -p 4000:4000 ghcr.io/scanii/scanii-cli:latest server
  ///
  /// Endpoint: http://localhost:4000  Key: key  Secret: secret
  /// </summary>
  [TestFixture]
  public class ScaniiClientTests
  {
    private static readonly string Endpoint =
      Environment.GetEnvironmentVariable("SCANII_ENDPOINT") ?? "http://localhost:4000";

    private const string Key = "key";
    private const string Secret = "secret";

    // scanii-cli local malware test fixture — safe to write to disk (not an AV signature)
    private const string MalwareContent = "38DCC0C9-7FB6-4D0D-9C37-288A380C6BB9";
    private const string MalwareFinding = "content.malicious.local-test-file";

    private IScaniiClient _client;
    private string _malwareFile;
    private string _cleanFile;

    [OneTimeSetUp]
    public void Setup()
    {
      _client = ScaniiClients.CreateDefault(Key, Secret,
        new HttpClient(), new ScaniiTarget(Endpoint));

      // malware test file — UUID recognized by scanii-cli, harmless to AV scanners
      _malwareFile = Path.GetTempFileName();
      File.WriteAllText(_malwareFile, MalwareContent);

      // clean file with random bytes
      _cleanFile = Path.GetTempFileName();
      var random = new byte[1024];
      new Random().NextBytes(random);
      File.WriteAllBytes(_cleanFile, random);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
      if (_malwareFile != null && File.Exists(_malwareFile))
        File.Delete(_malwareFile);
      if (_cleanFile != null && File.Exists(_cleanFile))
        File.Delete(_cleanFile);
    }

    [Test]
    public async Task ShouldPing()
    {
      Assert.That(await _client.Ping(), Is.True);
    }

    [Test]
    public async Task ShouldProcessMalwareFile()
    {
      var r = await _client.Process(_malwareFile);

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.Findings, Contains.Item(MalwareFinding));
      Assert.That(r.Findings.Count, Is.EqualTo(1));
      Assert.That(r.ContentLength, Is.GreaterThan(0));
      Assert.That(r.CreationDate, Is.Not.EqualTo(default(DateTime)));
      Assert.That(r.HostId, Is.Not.Null);
      Assert.That(r.RequestId, Is.Not.Null);
      Assert.That(r.ResourceLocation, Is.Not.Null);
    }

    [Test]
    public async Task ShouldProcessCleanFile()
    {
      var r = await _client.Process(_cleanFile);

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.Findings, Is.Empty);
    }

    [Test]
    public async Task ShouldProcessWithMetadata()
    {
      var r = await _client.Process(_malwareFile, metadata: new Dictionary<string, string>
      {
        {"foo", "bar"}
      });

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.Findings, Contains.Item(MalwareFinding));
      Assert.That(r.Metadata["foo"], Is.EqualTo("bar"));
      Assert.That(r.Metadata.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ShouldRetrievePreviousResult()
    {
      var original = await _client.Process(_malwareFile);
      var retrieved = await _client.Retrieve(original.ResourceId);

      Assert.That(retrieved.ResourceId, Is.EqualTo(original.ResourceId));
      Assert.That(retrieved.Findings, Contains.Item(MalwareFinding));
      Assert.That(retrieved.Checksum, Is.EqualTo(original.Checksum));
    }

    [Test]
    public async Task ShouldProcessAsyncWithoutCallback()
    {
      var r = await _client.ProcessAsync(_cleanFile);

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.ResourceLocation, Is.Not.Null);
      Assert.That(r.HostId, Is.Not.Null);
      Assert.That(r.RequestId, Is.Not.Null);

      var finalResult = TestUtils.PollForResult(() => _client.Retrieve(r.ResourceId));

      Assert.That(finalResult.ResourceId, Is.Not.Null);
      Assert.That(finalResult.Findings, Is.Empty);
    }

    [Test]
    public async Task ShouldProcessAsyncWithMetadata()
    {
      var r = await _client.ProcessAsync(_cleanFile, metadata: new Dictionary<string, string>
      {
        {"foo", "bar"}
      });

      Assert.That(r.ResourceId, Is.Not.Null);

      var finalResult = TestUtils.PollForResult(() => _client.Retrieve(r.ResourceId));

      Assert.That(finalResult.ResourceId, Is.Not.Null);
      Assert.That(finalResult.Metadata["foo"], Is.EqualTo("bar"));
    }

    [Test]
    public async Task ShouldCreateRetrieveAndDeleteAuthToken()
    {
      // create
      var token = await _client.CreateAuthToken(300);
      Assert.That(token.ResourceId, Is.Not.Null);
      Assert.That(token.CreationDate, Is.Not.EqualTo(default(DateTime)));
      Assert.That(token.ExpirationDate, Is.Not.EqualTo(default(DateTime)));

      // retrieve
      var token2 = await _client.RetrieveAuthToken(token.ResourceId);
      Assert.That(token2.ResourceId, Is.EqualTo(token.ResourceId));
      Assert.That(token2.ExpirationDate, Is.EqualTo(token.ExpirationDate));

      // delete
      await _client.DeleteAuthToken(token.ResourceId);
    }

    [Test]
    public async Task ShouldUseAuthTokenToCreateClient()
    {
      var token = await _client.CreateAuthToken(300);
      var tokenClient = ScaniiClients.CreateDefault(token,
        new HttpClient(), new ScaniiTarget(Endpoint));

      var r = await tokenClient.Process(_malwareFile);

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.Findings, Contains.Item(MalwareFinding));
    }

    [Test]
    public async Task ShouldFetchRemoteContent()
    {
      var r = await _client.Fetch("https://scanii.s3.amazonaws.com/eicarcom2.zip");

      Assert.That(r.ResourceId, Is.Not.Null);
      Assert.That(r.ResourceLocation, Is.Not.Null);
      Assert.That(r.HostId, Is.Not.Null);
      Assert.That(r.RequestId, Is.Not.Null);

      var finalResult = TestUtils.PollForResult(() => _client.Retrieve(r.ResourceId));

      Assert.That(finalResult.ResourceId, Is.Not.Null);
    }

    [Test]
    public async Task ShouldFetchRemoteContentWithMetadata()
    {
      var r = await _client.Fetch(
        "https://scanii.s3.amazonaws.com/eicarcom2.zip",
        metadata: new Dictionary<string, string> {{"hello", "world"}});

      Assert.That(r.ResourceId, Is.Not.Null);

      var finalResult = TestUtils.PollForResult(() => _client.Retrieve(r.ResourceId));

      Assert.That(finalResult.ResourceId, Is.Not.Null);
      Assert.That(finalResult.Metadata["hello"], Is.EqualTo("world"));
    }

    [Test]
    public void ShouldThrowAuthExceptionOnBadCredentials()
    {
      var badClient = ScaniiClients.CreateDefault("bad-key", "bad-secret",
        new HttpClient(), new ScaniiTarget(Endpoint));

      Assert.ThrowsAsync<ScaniiAuthException>(async () =>
        await badClient.Process(_cleanFile));
    }

    /// <summary>
    /// TODO: callback integration test — requires scanii-cli callback support.
    /// Once scanii-cli ships callback simulation, implement this test by spinning
    /// up a local HTTP server, passing its URL as the callback parameter, and
    /// asserting the payload.
    /// </summary>
    [Test]
    [Ignore("TODO: scanii-cli callback support not yet available — see RAFAEL_CHECKLIST.md §1.6")]
    public async Task ShouldDeliverCallbackOnProcess()
    {
      // stub — implement after scanii-cli adds callback simulation
      await Task.CompletedTask;
    }
  }
}
