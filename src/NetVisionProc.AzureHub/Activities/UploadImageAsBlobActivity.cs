using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Config;

namespace NetVisionProc.AzureHub.Activities;

public class UploadImageAsBlobActivity
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureHubConfig _config;

    public UploadImageAsBlobActivity(BlobServiceClient blobServiceClient, AzureHubConfig config)
    {
        _blobServiceClient = blobServiceClient;
        _config = config;
    }

    [FunctionName(nameof(UploadImageAsBlobActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        var sourceBlobData = context.GetInput<BlobUploadModel>();
        var sourceBlobClient = _blobServiceClient
            .GetBlobContainerClient(_config.BlobContainerName)
            .GetBlobClient(sourceBlobData.FileName);
        
        var imageBytes = Convert.FromBase64String(sourceBlobData.ImageData);
        using (var stream = new MemoryStream(imageBytes))
        {
            await sourceBlobClient.UploadAsync(stream, overwrite: true);
        }
    }
}