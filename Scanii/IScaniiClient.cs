using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Scanii.Models;

namespace Scanii
{
  /// <summary>
  /// Interface for a Scanii API client
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
  public interface IScaniiClient
  {
    /// <summary>
    ///   Submits a stream to be processed (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="contents">stream of the content to be analyzed</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this analysis</param>
    /// <returns>processing result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiProcessingResult> Process(Stream contents, string callback = null,
      Dictionary<string, string> metadata = null);

    /// <summary>
    ///   Submits a file to be processed (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="path">file path on the local system</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this analysis</param>
    /// <returns>processing result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiProcessingResult> Process(string path, string callback = null,
      Dictionary<string, string> metadata = null);

    /// <summary>
    ///   Submits a stream to be processed asynchronously (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="contents">stream of the content to be analyzed</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this analysis</param>
    /// <returns>pending result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiPendingResult> ProcessAsync(Stream contents, string callback = null,
      Dictionary<string, string> metadata = null);

    /// <summary>
    ///   Submits a file to be processed asynchronously (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="path">file path on the local system</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this analysis</param>
    /// <returns>pending result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiPendingResult> ProcessAsync(string path, string callback = null,
      Dictionary<string, string> metadata = null);

    /// <summary>
    ///   Fetches the results of a previously processed file (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="id">id of the content/file to be retrieved</param>
    /// <returns>ScaniiProcessingResult</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiProcessingResult> Retrieve(string id);

    /// <summary>
    ///   Pings the scanii service using the credentials provided (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <returns>true if ping was successful, false otherwise</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<bool> Ping();

    /// <summary>
    ///   Makes a fetch call to scanii (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="location">location (URL) of the content to be processed</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this file</param>
    /// <returns>pending result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiPendingResult> Fetch(string location, string callback = null,
      Dictionary<string, string> metadata = null);

    /// <summary>
    ///   Creates a new temporary authentication token (https://scanii.github.io/openapi/v22/)
    /// </summary>
    /// <param name="timeoutInSeconds">How long the token should be valid for</param>
    /// <returns>the new auth token</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiAuthToken> CreateAuthToken(int timeoutInSeconds = 300);

    /// <summary>
    ///   Deletes a previously created authentication token
    /// </summary>
    /// <param name="id">the id of the token to be deleted</param>
    /// <exception cref="ScaniiException"></exception>
    Task DeleteAuthToken(string id);

    /// <summary>
    ///   Retrieves a previously created auth token
    /// </summary>
    /// <param name="id">the id of the token to be retrieved</param>
    /// <returns>the auth token</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiAuthToken> RetrieveAuthToken(string id);

    /// <summary>
    ///   Retrieves the processing event trace for a previously scanned file (https://scanii.github.io/openapi/v22/).
    ///   This is preview surface in the v2.2 API — behavior may change in future releases.
    /// </summary>
    /// <param name="id">id of the previously scanned content</param>
    /// <returns>trace result, or null if the id is not found (404)</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiTraceResult> RetrieveTrace(string id);

    /// <summary>
    ///   Submits a remote URL for synchronous processing (https://scanii.github.io/openapi/v22/).
    ///   This is preview surface in the v2.2 API — behavior may change in future releases.
    /// </summary>
    /// <param name="location">URL of the content to be processed</param>
    /// <param name="callback">optional location (URL) to be notified and receive the result</param>
    /// <param name="metadata">optional metadata to be added to this analysis</param>
    /// <returns>processing result</returns>
    /// <exception cref="ScaniiException"></exception>
    Task<ScaniiProcessingResult> ProcessFromUrl(string location, string callback = null,
      Dictionary<string, string> metadata = null);
  }
}
