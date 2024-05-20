using Newtonsoft.Json;

namespace NetVisionProc.Application.AzureTableScope.Models;

public class ImagePredictionResult
{
    [JsonProperty("imageName")]
    public string ImageName { get; set; } = null!;
    
    [JsonProperty("resultImgName")]
    public string? ResultImgName { get; set; }
    
    [JsonProperty("resultImgPath")]
    public string? ResultImgPath { get; set; }

    [JsonProperty("detectorType")]
    public DetectorType DetectorType { get; set; }

    [JsonProperty("prediction")]
    public float Prediction { get; set; }

    [JsonProperty("errors")]
    public string? Errors { get; set; }

    [JsonProperty("hasErrors")]
    public bool HasErrors { get; set; }
    
    [JsonProperty("timeTaken")]
    public float TimeTaken { get; set; }
}

public enum DetectorType
{
    YoloV5 = 0,
    Ssd = 1,
    Unknown = 2
}