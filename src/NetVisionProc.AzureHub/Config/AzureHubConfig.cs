namespace NetVisionProc.AzureHub.Config
{
    public class AzureHubConfig
    {
        public const string SectionName = "AzureHubConfig";

        public string DbConnectionString { get; set; }

        public string BlobContainerName { get; set; }

        public string ProcessedBlobsContainerName { get; set; }

        public string TableStorageName { get; set; }

        public string QueueName { get; set; }

        public string BlobConnectionString { get; set; }

        public string TableConnectionString { get; set; }

        public string QueueConnectionString { get; set; }
    }
}