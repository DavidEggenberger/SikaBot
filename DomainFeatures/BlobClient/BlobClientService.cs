using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DomainFeatures.HubDocuments;

namespace DomainFeatures.BlobClient
{
    public class BlobClientService
    {
        private readonly IConfiguration configuration;
        private readonly BlobStorageConstants blobStorageConstants;
        public BlobClientService(IConfiguration configuration, BlobStorageConstants blobStorageConstants)
        {
            this.configuration = configuration;
            this.blobStorageConstants = blobStorageConstants;
        }

        public async Task DeleteAllBlobsAsync()
        {
            string storageConnectionString = configuration["BlobConnection"];

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = account.CreateCloudBlobClient();

            // Make sure container is there
            var blobContainer = blobClient.GetContainerReference("default");
            if (blobContainer != null && await blobContainer.ExistsAsync())
            {
                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                await blobContainer.DeleteIfExistsAsync();
            }
        }

        public async Task CreateAsync()
        {
            string storageConnectionString = configuration["BlobConnection"];

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = account.CreateCloudBlobClient();

            // Make sure container is there
            var blobContainer = blobClient.GetContainerReference("default");

            var deleted = false;

            while(deleted is false)
            {
                await Task.Delay(2500);
                try
                {
                    await blobContainer.CreateIfNotExistsAsync();
                }
                catch (Exception ex)
                {
                    continue;
                }
                deleted = true;
            }
        }
    }
}
