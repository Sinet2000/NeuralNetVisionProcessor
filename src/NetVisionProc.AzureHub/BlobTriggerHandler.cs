using System;
using System.IO;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetVisionProc.AzureHub.Activities;
using NetVisionProc.AzureHub.Config;
using TableEntity = Azure.Data.Tables.TableEntity;

namespace NetVisionProc.AzureHub
{
    public class BlobTriggerHandler
    {
        [FunctionName(nameof(ImageBlobUploadTrigger))]
        public static async Task ImageBlobUploadTrigger(
            [BlobTrigger("blob-py-test/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input
            // Start the orchestrator function
            string instanceId = await starter.StartNewAsync(nameof(ProcessImageBlobUploadedOrchestrator), input: name);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        [FunctionName(nameof(ProcessImageBlobUploadedOrchestrator))]
        public async Task ProcessImageBlobUploadedOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            string blobImageName = context.GetInput<string>();

            // Call the activity function
            await context.CallActivityAsync(nameof(InsertIntoTableStorageActivity), blobImageName);
            await context.CallActivityAsync(nameof(InsertEventToQueueStorageActivity), blobImageName);

            log.LogInformation($"Blob '{blobImageName}' processed.");
        }

        // [FunctionName(nameof(InsertIntoTableStorageActivity))]
        // public static async Task InsertIntoTableStorageActivity(
        //     [ActivityTrigger] string blobName,
        //     [Table("MyTable", Connection = "AzureWebJobsStorage")] CloudTable table,
        //     ILogger log)
        // {
        //     await table.CreateIfNotExistsAsync();
        //
        //     // Create a new table entity
        //     var entity = new DynamicTableEntity("PartitionKey", blobName)
        //     {
        //         Properties =
        //         {
        //             { "Message", new EntityProperty("Blob uploaded") }
        //         }
        //     };
        //
        //     // Insert the entity into the table
        //     var insertOperation = TableOperation.Insert(entity);
        //     await table.ExecuteAsync(insertOperation);
        //
        //     log.LogInformation($"Inserted entity for blob '{blobName}' into table storage.");
        // }

        // [FunctionName("ImageProcessingOrchestration")]
        // public static async Task RunImageProcessingOrc([OrchestrationTrigger] IDurableOrchestrationContext context)
        // {
        //     string name = context.GetInput<string>();
        //     if (string.IsNullOrEmpty(name))
        //     {
        //         throw new ArgumentNullException("Blob name is required.");
        //     }
        //     
        //     // Retrieve the image blob
        //     // var imageBlob = await context.CallActivityAsync<byte[]>("DownloadBlob", name);
        //
        //     // Process the image
        //     // var result = await context.CallActivityAsync<string>("ProcessImage", imageBlob);
        //
        //     // Store the result or perform any further actions
        // }
        //
        // [FunctionName("DownloadBlob")]
        // public static async Task<byte[]> DownloadBlob([ActivityTrigger] string blobName, ILogger log)
        // {
        //     try
        //     {
        //         // Replace "AzureWebJobsStorage" with your actual connection string name
        //         var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        //         var blobClient = storageAccount.CreateCloudBlobClient();
        //         var container = blobClient.GetContainerReference("images");
        //
        //         var blob = container.GetBlockBlobReference(blobName);
        //         using (var memoryStream = new MemoryStream())
        //         {
        //             await blob.DownloadToStreamAsync(memoryStream);
        //             return memoryStream.ToArray();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         log.LogError($"Error downloading blob {blobName}: {ex.Message}");
        //         throw;
        //     }
        // }
        //
        // [FunctionName("ProcessImage")]
        // public static async Task<string> ProcessImage([ActivityTrigger] byte[] imageBlob, ILogger log)
        // {
        //     try
        //     {
        //         // Perform image processing using YOLOv5 or other neural network
        //         // Example implementation:
        //         // var result = await YOLOv5.Process(imageBlob);
        //         // return result;
        //
        //         // For demonstration purposes, simply return a placeholder string
        //         return "Processed image result";
        //     }
        //     catch (Exception ex)
        //     {
        //         log.LogError($"Error processing image: {ex.Message}");
        //         throw;
        //     }
        // }
    }
}
