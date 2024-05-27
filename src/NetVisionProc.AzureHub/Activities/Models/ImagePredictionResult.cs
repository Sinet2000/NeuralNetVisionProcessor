using Newtonsoft.Json;

namespace NetVisionProc.AzureHub.Activities.Models;

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
    
    [JsonProperty("predictionClass")]
    public PredictionClass PredictionClass { get; set; }

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
    SSD = 1,
    Mask_R_CNN = 2,
    UNKNOWN = 3
}

public enum PredictionClass
{
    Basketball = 0,
    RedBall = 1,
    UNKNOWN = 99
}