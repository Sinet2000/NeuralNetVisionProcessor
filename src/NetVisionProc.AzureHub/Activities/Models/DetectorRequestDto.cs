using System.Text.Json.Serialization;

namespace NetVisionProc.AzureHub.Activities.Models;

public record DetectorRequestDto
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("sourceBlobUri")]
    public string SourceBlobUri { get; set; }
    
    [JsonPropertyName("detectorType")]
    public DetectorType DetectorType { get; set; }
}