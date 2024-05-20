using System.IO;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Config;
using NetVisionProc.AzureHub.Models;
using NetVisionProc.AzureHub.Models.Enums;
using NetVisionProc.AzureHub.TableEntities;

namespace NetVisionProc.AzureHub.Activities;

public class CopyBlobToDestinationActivity
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureHubConfig _config;
    private readonly TableClient _tableClient;

    public CopyBlobToDestinationActivity(
        BlobServiceClient blobServiceClient,
        AzureHubConfig azureHubConfig,
        TableClient tableClient)
    {
        _config = azureHubConfig;
        _blobServiceClient = blobServiceClient;
        _tableClient = tableClient;
    }

    [FunctionName(nameof(CopyBlobToDestinationActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        var sourceBlobData = context.GetInput<BlobInputModel>();
        try
        {
            var sourceBlobClient = _blobServiceClient
                .GetBlobContainerClient(_config.BlobContainerName)
                .GetBlobClient(sourceBlobData.Name);
            var sourceBlobUri = sourceBlobClient.Uri.AbsoluteUri;
            
            var destinationBlobContainer = _blobServiceClient
                .GetBlobContainerClient(_config.ProcessedBlobsContainerName);
            await destinationBlobContainer.CreateIfNotExistsAsync();

            var destBlobClient = destinationBlobContainer.GetBlobClient(sourceBlobData.Name);
            await destBlobClient.UploadAsync(new MemoryStream(sourceBlobData.StreamData), overwrite: true);
            var destinationBlobUri = destBlobClient.Uri.AbsoluteUri;
            
            log.LogInformation($"Uploaded blob '{sourceBlobData.Name}' to destination container.");
            
            await _tableClient.CreateIfNotExistsAsync();

            var processedBlobDataModel = new ProcessedBlobModel(
                sourceBlobUri,
                sourceBlobData.Name,
                BlobProcessStatus.ReadyToProcess);

            // var processedBlobTableEntity = new ImagePredictionResultTableEntity(processedBlobDataModel);
            // var entityToInsert = new TableEntity(processedBlobTableEntity.PartitionKey, processedBlobTableEntity.RowKey);
            // foreach (var kvp in processedBlobTableEntity.GetPropertiesDictionary())
            // {
            //     entityToInsert.Add(kvp.Key, kvp.Value);
            // }
            //
            // await _tableClient.AddEntityAsync(entityToInsert);
        }
        catch (RequestFailedException ex)
        {
            log.LogError($"Failed to upload blob '{sourceBlobData.Name}': {ex.Message}");
        }
    }
}