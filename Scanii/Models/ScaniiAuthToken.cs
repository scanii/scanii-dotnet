using System;
using System.Text.Json.Serialization;

namespace Scanii.Models
{
  public class ScaniiAuthToken : ScaniiResult
  {
    [JsonPropertyName("id")] public string ResourceId { get; set; }

    [JsonPropertyName("creation_date")] public DateTime CreationDate { get; set; }

    [JsonPropertyName("expiration_date")] public DateTime ExpirationDate { get; set; }
  }
}
