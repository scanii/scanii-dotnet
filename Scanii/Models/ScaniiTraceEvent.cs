using System;
using System.Text.Json.Serialization;

namespace Scanii.Models
{
  public class ScaniiTraceEvent
  {
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
  }
}
