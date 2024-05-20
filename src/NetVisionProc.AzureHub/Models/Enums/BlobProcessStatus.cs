namespace NetVisionProc.AzureHub.Models.Enums;

public enum BlobProcessStatus : byte
{
    ReadyToProcess = 0,
    InProgress = 1,
    Processed = 2,
    ErrorOccurred = 3
}