namespace NetVisionProc.AzureHub.Config;

public record NeuralVisionSystemConfig
{
    public const string SectionName = "NeuralVisionSystemConfig";
    
    public string YolovSystemUri { get; set; }
    
    public string SsdSystemUri { get; set; }
    
    public string MaskRCnnSystemUri { get; set; }
}