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
    private readonly NeuralVisionSystemConfig _neuralVisionSystemConfig;

    public RunImageDetectionActivity(
        BlobServiceClient blobServiceClient,
        AzureHubConfig config,
        NeuralVisionSystemConfig neuralVisionSystemConfig,
        TableClient tableClient)
    {
        _blobServiceClient = blobServiceClient;
        _config = config;
        _neuralVisionSystemConfig = neuralVisionSystemConfig;
    }

    [FunctionName(nameof(RunImageDetectionActivity))]
    public async Task<ImagePredictionResult> Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        var detectorInput = context.GetInput<DetectorInput>();

        try
        {
            return await GetImagePredictionResultFromDetector(log, detectorInput);
        }
        catch (RequestFailedException ex)
        {
            log.LogError($"Failed to process blob '{detectorInput.ImageName}': {ex.Message}");
            return new ImagePredictionResult()
            {
                DetectorType = detectorInput.DetectorType,
                Errors = ex.Message,
                HasErrors = true,
                ImageName = detectorInput.ImageName
            };
        }
    }

    private async Task<ImagePredictionResult> GetImagePredictionResultFromDetector(ILogger logger,
        DetectorInput detectorInput)
    {
        var sourceBlobClient = _blobServiceClient
            .GetBlobContainerClient(_config.BlobContainerName)
            .GetBlobClient(detectorInput.ImageName);

        var reqData = new DetectorRequestDto
        {
            FileName = detectorInput.ImageName,
            SourceBlobUri = sourceBlobClient.Uri.AbsoluteUri,
            DetectorType = detectorInput.DetectorType
        };

        var detectorUriString = detectorInput.DetectorType switch
        {
            DetectorType.YoloV5 => _neuralVisionSystemConfig.YolovSystemUri,
            DetectorType.SSD => _neuralVisionSystemConfig.SsdSystemUri,
            DetectorType.Mask_R_CNN => _neuralVisionSystemConfig.MaskRCnnSystemUri,
            DetectorType.UNKNOWN => throw new ArgumentException(
                $"Such detector is not found, only: {DetectorType.YoloV5.ToString()}, {DetectorType.SSD.ToString()}, {DetectorType.Mask_R_CNN.ToString()} are allowed"),
            _ => throw new ArgumentOutOfRangeException(nameof(detectorInput.DetectorType), detectorInput.DetectorType, null)
        };

        using var httpClient = new HttpClient();
        var uri = new Uri(detectorUriString);
        var jsonString = JsonSerializer.Serialize(reqData);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        logger.LogInformation($"{nameof(RunImageDetectionActivity)}.Response: {responseString}");

        var imagePredictionResult = JsonConvert.DeserializeObject<ImagePredictionResult>(responseString);

        return imagePredictionResult ?? new ImagePredictionResult
        {
            DetectorType = detectorInput.DetectorType,
            Errors = "The response from detector cannot be recognized",
            HasErrors = true
        };
    }
}