using System;
using System.IO;
using Azure.Storage.Blobs;
using NetVisionProc.Common.Extensions;
using NetVisionProc.Domain.Enums;
using Throw;

namespace NetVisionProc.AzureHub.Utils
{
    public class AzureBlobUtil
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
    }
}