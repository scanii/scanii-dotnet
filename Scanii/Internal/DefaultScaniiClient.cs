using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Scanii.Models;

namespace Scanii.Internal
{
  public class DefaultScaniiClient : IScaniiClient
  {
    private readonly HttpClient _httpClient;

    public DefaultScaniiClient(ScaniiTarget target, string key, string secret, HttpClient httpClient)
    {
      if (key == null) throw new ArgumentNullException(nameof(key));
      if (key.Length == 0) throw new ArgumentException("API key cannot be the empty string.", nameof(key));

      _httpClient = httpClient;
      ConfigureClient(target, key, secret);
    }

    public async Task<ScaniiProcessingResult> Process(Stream contents, string callback = null,
      Dictionary<string, string> metadata = null)
    {
      var formDataContent = new MultipartFormDataContent {{new StreamContent(contents), "file"}};

      if (metadata != null)
        foreach (var keyValuePair in metadata)
          formDataContent.Add(new StringContent(keyValuePair.Value), $"metadata[{keyValuePair.Key}]");

      if (callback != null) formDataContent.Add(new StringContent(callback), "callback");

      using var response = await _httpClient.PostAsync("/v2.2/files", formDataContent);
      Trace.WriteLine($"[scanii] POST /v2.2/files status={response.StatusCode}");

      CheckForErrors(response);

      if (response.StatusCode == HttpStatusCode.Created)
        return DecorateEntity(
          await JsonSerializer.DeserializeAsync<ScaniiProcessingResult>(await response.Content.ReadAsStreamAsync()),
          response);

      var responseBody = await response.Content.ReadAsStringAsync();
      throw new ScaniiException(
        $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
    }

    public Task<ScaniiProcessingResult> Process(string path, string callback = null,
      Dictionary<string, string> metadata = null)
    {
      return Process(new MemoryStream(File.ReadAllBytes(path)), callback, metadata);
    }

    public async Task<ScaniiPendingResult> ProcessAsync(Stream contents, string callback = null,
      Dictionary<string, string> metadata = null)
    {
      var req = new MultipartFormDataContent {{new StreamContent(contents), "file"}};

      if (metadata != null)
        foreach (var keyValuePair in metadata)
          req.Add(new StringContent(keyValuePair.Value), $"metadata[{keyValuePair.Key}]");

      if (callback != null) req.Add(new StringContent(callback), "callback");

      using var response = await _httpClient.PostAsync("/v2.2/files/async", req);
      Trace.WriteLine($"[scanii] POST /v2.2/files/async status={response.StatusCode}");

      CheckForErrors(response);

      if (response.StatusCode == HttpStatusCode.Accepted)
        return DecorateEntity(
          await JsonSerializer.DeserializeAsync<ScaniiPendingResult>(await response.Content.ReadAsStreamAsync()),
          response);

      var responseBody = await response.Content.ReadAsStringAsync();
      throw new ScaniiException(
        $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
    }

    public Task<ScaniiPendingResult> ProcessAsync(string path, string callback = null,
      Dictionary<string, string> metadata = null)
    {
      return ProcessAsync(new MemoryStream(File.ReadAllBytes(path)), callback, metadata);
    }

    public async Task<ScaniiProcessingResult> Retrieve(string id)
    {
      using var response = await _httpClient.GetAsync($"/v2.2/files/{id}");

      CheckForErrors(response);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new ScaniiException(
          $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
      }

      var body = await response.Content.ReadAsStringAsync();
      Trace.WriteLine($"[scanii] GET /v2.2/files/{id} body={body}");
      return DecorateEntity(
        JsonSerializer.Deserialize<ScaniiProcessingResult>(body),
        response);
    }

    public async Task<bool> Ping()
    {
      using var response = await _httpClient.GetAsync("/v2.2/ping");
      if (response.StatusCode != HttpStatusCode.OK)
        throw new ScaniiException(
          $"Invalid HTTP response from service, code: {response.StatusCode}");

      return true;
    }

    public async Task<ScaniiPendingResult> Fetch(string location, string callback = null,
      Dictionary<string, string> metadata = null)
    {
      if (location == null) throw new ArgumentNullException(nameof(location));

      var parameters = new Dictionary<string, string> {{"location", location}};

      if (callback != null) parameters.Add("callback", callback);

      if (metadata != null)
        foreach (var keyValuePair in metadata)
          parameters.Add($"metadata[{keyValuePair.Key}]", keyValuePair.Value);

      using var response = await _httpClient.PostAsync("/v2.2/files/fetch", new FormUrlEncodedContent(parameters));
      Trace.WriteLine($"[scanii] POST /v2.2/files/fetch status={response.StatusCode}");

      CheckForErrors(response);

      if (response.StatusCode == HttpStatusCode.Accepted)
        return DecorateEntity(
          await JsonSerializer.DeserializeAsync<ScaniiPendingResult>(await response.Content.ReadAsStreamAsync()),
          response);

      var responseBody = await response.Content.ReadAsStringAsync();
      throw new ScaniiException(
        $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
    }

    public async Task<ScaniiAuthToken> CreateAuthToken(int timeoutInSeconds = 300)
    {
      var parameters = new Dictionary<string, string> {{"timeout", timeoutInSeconds.ToString()}};

      var req = new FormUrlEncodedContent(parameters);
      using var response = await _httpClient.PostAsync("/v2.2/auth/tokens", req);

      CheckForErrors(response);

      if (response.StatusCode == HttpStatusCode.Created)
        return DecorateEntity(
          await JsonSerializer.DeserializeAsync<ScaniiAuthToken>(await response.Content.ReadAsStreamAsync()),
          response);

      var responseBody = await response.Content.ReadAsStringAsync();
      throw new ScaniiException(
        $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
    }

    public async Task DeleteAuthToken(string id)
    {
      using var response = await _httpClient.DeleteAsync($"/v2.2/auth/tokens/{id}");

      CheckForErrors(response);

      if (response.StatusCode == HttpStatusCode.NoContent) return;

      var responseBody = await response.Content.ReadAsStringAsync();
      throw new ScaniiException(
        $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
    }

    public async Task<ScaniiAuthToken> RetrieveAuthToken(string id)
    {
      using var response = await _httpClient.GetAsync($"/v2.2/auth/tokens/{id}");

      CheckForErrors(response);

      if (response.StatusCode != HttpStatusCode.OK)
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new ScaniiException(
          $"Invalid HTTP response from service, code: {response.StatusCode} message: {responseBody}");
      }

      return DecorateEntity(
        await JsonSerializer.DeserializeAsync<ScaniiAuthToken>(await response.Content.ReadAsStreamAsync()),
        response);
    }

    private static void CheckForErrors(HttpResponseMessage response)
    {
      if (response.StatusCode == HttpStatusCode.Unauthorized)
        throw new ScaniiAuthException("Invalid credentials — check your API key and secret.");

      if (response.StatusCode == (HttpStatusCode)429)
        throw new ScaniiRateLimitException("Rate limit exceeded. Check the Retry-After header.");
    }

    private void ConfigureClient(ScaniiTarget target, string key, string secret)
    {
      var version = Assembly.GetExecutingAssembly().GetName().Version;

      _httpClient.BaseAddress = target.Endpoint;
      _httpClient.DefaultRequestHeaders.Add("User-Agent", $"{HttpHeaders.UserAgent}/v{version}");
      _httpClient.DefaultRequestHeaders.Add("Authorization",
        "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}")));

      Trace.WriteLine($"[scanii] client initialized version={version} endpoint={target.Endpoint}");
    }

    private static T DecorateEntity<T>(T entity, HttpResponseMessage response) where T : ScaniiResult
    {
      entity.StatusCode = response.StatusCode.GetHashCode();
      if (response.Headers.Contains(HttpHeaders.XHostHeader))
        entity.HostId = response.Headers.GetValues(HttpHeaders.XHostHeader).First();

      if (response.Headers.Contains(HttpHeaders.Location))
        entity.ResourceLocation = response.Headers.GetValues(HttpHeaders.Location).First();

      if (response.Headers.Contains(HttpHeaders.XRequestHeader))
        entity.RequestId = response.Headers.GetValues(HttpHeaders.XRequestHeader).First();

      return entity;
    }
  }
}
