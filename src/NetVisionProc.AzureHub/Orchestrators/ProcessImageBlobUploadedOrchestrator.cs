using System.Collections.Generic;
using System.Linq;
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
            
            var imagePredictionsTasks = new List<Task<ImagePredictionResult>>();
            
            var yolovPredictionResult = context.CallActivityAsync<ImagePredictionResult>(
                nameof(RunImageDetectionActivity),
                new DetectorInput(blobInput.Name, DetectorType.YoloV5)
                );
            imagePredictionsTasks.Add(yolovPredictionResult);
            
            // var ssdPredictionResult = context.CallActivityAsync<ImagePredictionResult>(
            //     nameof(RunImageDetectionActivity),
            //     new DetectorInput(blobInput.Name, DetectorType.SSD)
            //     );
            // imagePredictionsTasks.Add(ssdPredictionResult);
            //
            // var maskRCnnPredictionResult = context.CallActivityAsync<ImagePredictionResult>(
            //     nameof(RunImageDetectionActivity),
            //     new DetectorInput(blobInput.Name, DetectorType.Mask_R_CNN)
            //     );
            // imagePredictionsTasks.Add(maskRCnnPredictionResult);
            
            await Task.WhenAll(imagePredictionsTasks);

            var imagePredictionResults = imagePredictionsTasks.Select(t => t.Result).ToList();
            
            await context.CallActivityAsync(nameof(InsertDetectionResultsToTableActivity), imagePredictionResults);

            log.LogInformation($"Blob '{blobInput.Name}' processed.");
        }
    }
}