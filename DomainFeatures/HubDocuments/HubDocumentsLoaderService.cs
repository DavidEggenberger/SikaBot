using DomainFeatures.HubDocuments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using DomainFeatures.HubDocuments.Domain;
using DomainFeatures.HubDocuments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using UglyToad.PdfPig.Core;

namespace DomainFeatures.Database
{
    public class HubDocumentsLoaderService
    {
        private readonly HubDocumentsSingleton hubDocumentsSingleton;
        private readonly ImageAnalyzerService imageAnalyzerService;
        private readonly IConfiguration configuration;
        public HubDocumentsLoaderService(IConfiguration configuration, HubDocumentsSingleton hubDocumentsSingleton, ImageAnalyzerService imageAnalyzerService)
        {
            this.hubDocumentsSingleton = hubDocumentsSingleton;
            this.imageAnalyzerService = imageAnalyzerService;
            this.configuration = configuration;
        }
        public async Task LoadHubDocumentsAsnyc()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Data\PDF\"), "*.pdf", SearchOption.AllDirectories))
            {
                HubDocument hubDocument = new HubDocument();
                using (PdfDocument document = PdfDocument.Open(file))
                {
                    string text = string.Empty;
                    foreach (Page page in document.GetPages())
                    {
                        if (text.Length < 2500)
                        {
                            text += $" {page.Text}";
                        }

                        var images = page.GetImages().Take(1);
                        foreach (var image in images)
                        {
                            await imageAnalyzerService.AnalyzeImage(hubDocument, image.RawBytes.ToArray());
                        }
                    }

                    hubDocument.Location = file;
                    hubDocument.Text = text;
                    hubDocument.Id = Guid.NewGuid();

                    hubDocumentsSingleton.HubDocuments.Add(hubDocument);
                }

                string storageConnectionString = configuration["BlobConnection"];

                CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = account.CreateCloudBlobClient();

                // Make sure container is there
                var blobContainer = blobClient.GetContainerReference("default");

                var idd = Guid.NewGuid();

                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference($"{idd.ToString()}.pdf");
                await blockBlob.UploadFromStreamAsync(File.OpenRead(file));
                hubDocument.Uri = blockBlob.Uri.AbsoluteUri;
            }
        }
    }
}
