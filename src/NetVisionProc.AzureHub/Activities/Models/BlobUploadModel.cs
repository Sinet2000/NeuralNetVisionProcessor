using Newtonsoft.Json;

namespace NetVisionProc.AzureHub.Activities.Models;

public record BlobUploadModel
{
    [JsonProperty("imageData")]
    public string ImageData { get; set; }
    
    [JsonProperty("fileName")]
    public string FileName { get; set; }
}