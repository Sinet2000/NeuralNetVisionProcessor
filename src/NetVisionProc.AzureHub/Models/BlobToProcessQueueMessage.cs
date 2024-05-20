using NetVisionProc.AzureHub.Models.Enums;

namespace NetVisionProc.AzureHub.Models;

public record BlobToProcessQueueMessage(string PartitionKey, string RowKey, string SourceUri, string FileName, BlobProcessStatus ProcessStatus);