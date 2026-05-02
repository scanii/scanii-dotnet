# scanii-dotnet

.NET client for the [Scanii](https://scanii.com) content processing API.

## Installation

```
dotnet add package Scanii --version 7.1.0
```

## SDK Principles

1. **Light.** Zero runtime dependencies, stdlib only.
2. **Up to date.** Always current with the latest Scanii API.
3. **Integration-only.** Wraps the REST API — retries, concurrency, and batching are the caller's responsibility.

## Quickstart

```csharp
using Scanii;

var client = ScaniiClients.CreateDefault("your-api-key", "your-api-secret");
var result = await client.Process("/path/to/file.pdf");
if (result.Findings.Count == 0)
    Console.WriteLine("Content is safe!");
else
    Console.WriteLine($"Findings: {string.Join(", ", result.Findings)}");
```

## API Reference

Full API documentation: <https://scanii.github.io/openapi/v22/>

All methods are on `IScaniiClient`, created via `ScaniiClients.CreateDefault`.

| Method | Description |
|---|---|
| `Process(path/stream, callback?, metadata?)` | Synchronous file scan |
| `ProcessAsync(path/stream, callback?, metadata?)` | Server-side async scan, returns pending result |
| `ProcessFromUrl(url, callback?, metadata?)` | Synchronous scan of a remote URL (v2.2 preview) |
| `Fetch(url, callback?, metadata?)` | Server-side fetch-and-scan of a remote URL |
| `Retrieve(id)` | Retrieve a previous scan result |
| `RetrieveTrace(id)` | Retrieve processing event trace; returns `null` on 404 (v2.2 preview) |
| `CreateAuthToken(timeoutSeconds)` | Mint a short-lived auth token |
| `RetrieveAuthToken(id)` | Inspect an auth token |
| `DeleteAuthToken(id)` | Revoke an auth token |
| `Ping()` | Health check |

## Regional Endpoints

```csharp
// Default (auto-routed)
var client = ScaniiClients.CreateDefault(key, secret);

// Regional
var client = ScaniiClients.CreateDefault(key, secret, target: ScaniiTarget.Eu1);
```

| Target | Endpoint |
|---|---|
| `ScaniiTarget.Auto` | `https://api.scanii.com` |
| `ScaniiTarget.Us1` | `https://api-us1.scanii.com` |
| `ScaniiTarget.Ca1` | `https://api-ca1.scanii.com` |
| `ScaniiTarget.Eu1` | `https://api-eu1.scanii.com` |
| `ScaniiTarget.Eu2` | `https://api-eu2.scanii.com` |
| `ScaniiTarget.Ap1` | `https://api-ap1.scanii.com` |
| `ScaniiTarget.Ap2` | `https://api-ap2.scanii.com` |

## Error Handling

```csharp
try
{
    var result = await client.Process("/path/to/file");
}
catch (ScaniiAuthException)
{
    // HTTP 401 — bad credentials
}
catch (ScaniiRateLimitException)
{
    // HTTP 429 — rate limit hit
}
catch (ScaniiException ex)
{
    // other API errors
}
```

## Local Testing with scanii-cli

```bash
docker run -d --name scanii-cli -p 4000:4000 ghcr.io/scanii/scanii-cli:latest server
```

```csharp
var client = ScaniiClients.CreateDefault("key", "secret",
    target: new ScaniiTarget("http://localhost:4000"));
```

## License

Apache 2.0. See [LICENSE](LICENSE).
