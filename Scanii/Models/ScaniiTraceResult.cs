using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Scanii.Models
{
  public class ScaniiTraceResult : ScaniiResult
  {
    [JsonPropertyName("id")] public string ResourceId { get; set; }
    [JsonPropertyName("events")] public List<ScaniiTraceEvent> Events { get; set; } = new List<ScaniiTraceEvent>();
  }
}
