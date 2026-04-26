using System.Text.Json.Serialization;

namespace Scanii.Models
{
  public class ScaniiPendingResult : ScaniiResult
  {
    [JsonPropertyName("id")] public string ResourceId { get; set; }
  }
}
