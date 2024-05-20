using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities;
using NetVisionProc.AzureHub.Activities.Models;

namespace NetVisionProc.AzureHub.Orchestrators
{
    public class ProcessImageBlobUploadedOrchestrator
    {
        [FunctionName(nameof(ProcessImageBlobUploadedOrchestrator))]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var blobInput = context.GetInput<BlobInputModel>();
            var yolovPredictionResult = await context.CallActivityAsync<ImagePredictionResult>(nameof(RunImageDetectionActivity), blobInput);
            await context.CallActivityAsync(nameof(InsertDetectionResultToTableActivity), yolovPredictionResult);

            log.LogInformation($"Blob '{blobInput.Name}' processed.");
        }
    }
}