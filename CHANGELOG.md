## [Unreleased]

* Dropped the "v2.2 preview" designation from `RetrieveTrace` and `ProcessFromUrl`. The trace endpoint is no longer marked preview in the contract, and `ProcessFromUrl` was never preview; the methods themselves are unchanged.

## [7.3.1]

* Added `Delete(id)` — deletes the processing result for a previously scanned file (`DELETE /v2.2/files/{id}`); the trace is preserved. Returns `true` on HTTP 204, throws `ScaniiException` on 404.
* Added `DeleteTrace(id)` — deletes the processing event trace for a previously scanned file (`DELETE /v2.2/files/{id}/trace`). Returns `true` on HTTP 204, throws `ScaniiException` on 404.
* Added integration tests: result deletion, trace-remains-after-result-deletion, trace deletion, and unknown-ID error behavior.

## [7.2.1]

* Bumped test dependencies: NUnit 4.3.2 → 4.6.1, NUnit3TestAdapter 4.6.0 → 6.2.0, Microsoft.NET.Test.Sdk 17.14.1 → 18.9.0, Serilog 4.3.1 → 4.4.0, Serilog.Extensions.Logging 9.0.2 → 10.0.0, Microsoft.Extensions.Logging.Abstractions 9.0.17 → 10.0.11. No runtime dependencies — the published package is unchanged.
* Bumped CI action: `actions/setup-dotnet` v5 → v6.

## [7.2.0]

* `ScaniiTarget.Auto` marked `[Obsolete]` — use an explicit regional target (`Us1`, `Eu1`, `Eu2`, `Ap1`, `Ap2`, `Ca1`) for data residency compliance. Will be removed in a future major version.

## [7.1.0]

* Added `RetrieveTrace(id)` — retrieves the processing event trace for a previously scanned file (`GET /v2.2/files/{id}/trace`); returns `null` on 404. Preview surface per the v2.2 API spec.
* Added `ProcessFromUrl(location, callback?, metadata?)` — synchronous scan of a remote URL (`POST /v2.2/files` with `location` form field). Preview surface per the v2.2 API spec.

## 7.0.0

* **Breaking:** NuGet package renamed from `UvaSoftware.Scanii` to `Scanii`. Install: `dotnet add package Scanii`
* **Breaking:** Root namespace renamed from `UvaSoftware.Scanii` to `Scanii`
* **Breaking:** Entity namespace renamed from `UvaSoftware.Scanii.Entities` to `Scanii.Models`
* **Breaking:** `ScaniiClients.CreateDefault` no longer accepts an `ILogger` parameter (logging moved to `System.Diagnostics.Trace`)
* Dropped all runtime dependencies: `Serilog`, `Microsoft.Extensions.Logging.Abstractions`, explicit `System.Text.Json`, explicit `System.Net.Http`
* Added `ScaniiAuthException` (HTTP 401) and `ScaniiRateLimitException` (HTTP 429) exception types
* Updated API endpoints from v2.1 to v2.2
* Added `ScaniiTarget.Ca1` regional endpoint
* Integration tests now run against scanii-cli — no real credentials required
* CI matrix updated to .NET 8 LTS and .NET 10 LTS across Ubuntu, macOS, and Windows

Migration from `UvaSoftware.Scanii`:

```bash
dotnet remove package UvaSoftware.Scanii
dotnet add package Scanii --version 7.0.0
```

Update namespace imports:
```csharp
// Before
using UvaSoftware.Scanii;
using UvaSoftware.Scanii.Entities;

// After
using Scanii;
using Scanii.Models;
```

## v4.0.2
* Dropped RestSharp in favor of native HttpClient and made HttpClient configurable
* Dropped Serilog in favor of MS.Extensions.Logging.Abstraction (https://github.com/uvasoftware/scanii-dotnet/issues/17)
* Extracted main class into an interface IScaniiClient
* Added Stream support (https://github.com/uvasoftware/scanii-dotnet/issues/16)
* Extended tests suite including multiple .net runtimes and OSs
