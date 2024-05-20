using System;
using System.Net.Http;
using System.Text;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Config;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NetVisionProc.AzureHub.Activities;

public class RunImageDetectionActivity
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureHubConfig _config;
    
    public RunImageDetectionActivity(
        BlobServiceClient blobServiceClient,
        AzureHubConfig config,
        TableClient tableClient)
    {
        _blobServiceClient = blobServiceClient;
        _config = config;
    }

    [FunctionName(nameof(RunImageDetectionActivity))]
    public async Task<ImagePredictionResult> Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        var sourceBlobData = context.GetInput<BlobInputModel>();
        var sourceBlobClient = _blobServiceClient
            .GetBlobContainerClient(_config.BlobContainerName)
            .GetBlobClient(sourceBlobData.Name);
        var sourceBlobUri = sourceBlobClient.Uri.AbsoluteUri;
        
        try
        {
            var reqData = new
            {
                fileName = sourceBlobData.Name,
                sourceUri = sourceBlobUri,
                detectorType = 0
            };
            
            using var httpClient = new HttpClient();
            string uriString = "http://localhost:7071/api/detectors";
            Uri uri = new Uri(uriString);
            var jsonString = JsonSerializer.Serialize(reqData);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            log.LogInformation($"{nameof(RunImageDetectionActivity)}.Response: {responseString}");
                
            var imagePredictionResult = JsonConvert.DeserializeObject<ImagePredictionResult>(responseString);

            return imagePredictionResult;
        }
        catch (RequestFailedException ex)
        {
            log.LogError($"Failed to process blob '{sourceBlobData.Name}': {ex.Message}");
            return new ImagePredictionResult()
            {
                DetectorType = DetectorType.YoloV5,
                Errors = ex.Message,
                HasErrors = true,
                ImageName = sourceBlobData.Name
            };
        }
    }
}