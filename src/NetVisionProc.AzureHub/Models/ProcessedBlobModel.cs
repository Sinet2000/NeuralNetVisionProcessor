using NetVisionProc.AzureHub.Models.Enums;

namespace NetVisionProc.AzureHub.Models
{
    public record ProcessedBlobModel(string SourceUri, string FileName, BlobProcessStatus ProcessStatus);
}