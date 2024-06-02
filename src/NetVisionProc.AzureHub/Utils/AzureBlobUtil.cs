using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NetVisionProc.Common;
using NetVisionProc.Common.Extensions;
using Throw;

namespace NetVisionProc.AzureHub.Utils
{
    public static class AzureBlobUtil
    {
        public static bool IsBlobImage(BlobClient blobClient)
        {
            var blobExtension = Path.GetExtension(blobClient.Name)?.ToLower();
            if (blobExtension.IsNullOrEmpty())
            {
                return false;
            }

            return blobExtension.Equals(FileExtensions.Jpeg.GetDescription(), StringComparison.OrdinalIgnoreCase)
                   || blobExtension.Equals(FileExtensions.Jpg.GetDescription(), StringComparison.OrdinalIgnoreCase)
                   || blobExtension.Equals(FileExtensions.Png.GetDescription(), StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool IsBlobImage(string blobName)
        {
            blobName.Throw().IfEmpty();
            var blobExtension = Path.GetExtension(blobName).ToLower();
            if (blobExtension.IsNullOrEmpty())
            {
                return false;
            }

            return blobExtension.Equals(FileExtensions.Jpeg.GetDescription(), StringComparison.OrdinalIgnoreCase)
                   || blobExtension.Equals(FileExtensions.Jpg.GetDescription(), StringComparison.OrdinalIgnoreCase)
                   || blobExtension.Equals(FileExtensions.Png.GetDescription(), StringComparison.OrdinalIgnoreCase);
        }
        
        public static async Task CheckAndCreateBlobContainer(string connectionString, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Check if the container exists
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            bool containerExists = await blobContainerClient.ExistsAsync();

            if (!containerExists)
            {
                // Create the container
                await blobContainerClient.CreateAsync(PublicAccessType.BlobContainer);
            }
        }
    }
}