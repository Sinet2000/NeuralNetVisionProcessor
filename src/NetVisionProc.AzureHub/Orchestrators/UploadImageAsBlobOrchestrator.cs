using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities;
using NetVisionProc.AzureHub.Activities.Models;

namespace NetVisionProc.AzureHub.Orchestrators;

public class UploadImageAsBlobOrchestrator
{
    [FunctionName(nameof(UploadImageAsBlobOrchestrator))]
    public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
    {
        var blobUploadModel = context.GetInput<BlobUploadModel>();
        await context.CallActivityAsync<BlobUploadModel>(nameof(UploadImageAsBlobActivity), blobUploadModel);
    }
}