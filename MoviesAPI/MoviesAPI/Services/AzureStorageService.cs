﻿using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public class AzureStorageService : IFileStorageService
    {
        private readonly string _connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorageConnection");
        }

        public async Task DeleteFile(string fileRoute, string containerName)
        {
            if (fileRoute != null)
            {
                var account = CloudStorageAccount.Parse(_connectionString);
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference(containerName);

                var blobName = Path.GetFileName(fileRoute);
                var blob = container.GetBlockBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute, string contentType)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(content, extension, containerName, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName, string contentType)
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadFromByteArrayAsync(content, 0, content.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }
    }
}