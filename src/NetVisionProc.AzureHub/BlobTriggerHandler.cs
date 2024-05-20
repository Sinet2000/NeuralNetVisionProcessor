using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Orchestrators;
using NetVisionProc.AzureHub.Utils;
using Newtonsoft.Json;

namespace NetVisionProc.AzureHub
{
    public static class BlobTriggerHandler
    {
        [FunctionName(nameof(UploadAsBlobTrigger))]
        public static async Task<IActionResult> UploadAsBlobTrigger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var blobUploadModel = JsonConvert.DeserializeObject<BlobUploadModel>(requestBody);
            
            string instanceId = await starter.StartNewAsync<BlobUploadModel>(
                nameof(UploadImageAsBlobOrchestrator), blobUploadModel);
            
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        
        [FunctionName(nameof(ImageBlobUploadTrigger))]
        public static async Task ImageBlobUploadTrigger(
            [BlobTrigger("visionprocessor-origin/{name}", Connection = "AzureWebJobsStorage")] Stream imageBlob,
            string name,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            if (AzureBlobUtil.IsBlobImage(name))
            {
                byte[] blobBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    await imageBlob.CopyToAsync(ms);
                    blobBytes = ms.ToArray();
                }

                // Pass the byte array to BlobInputModel constructor
                string instanceId = await starter.StartNewAsync<BlobInputModel>(
                    nameof(ProcessImageBlobUploadedOrchestrator), new BlobInputModel(blobBytes, name));

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            }
            else
            {
                log.LogInformation($"The blob '{name}' is not an image.");
            }
        }
    }
}
