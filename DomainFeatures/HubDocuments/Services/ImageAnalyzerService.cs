using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace DomainFeatures.HubDocuments.Services
{
    public class ImageAnalyzerService
    {
        private readonly IConfiguration configuration;
        private readonly VisionServiceOptions visionServiceOptions;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ImageAnalyzerService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
            visionServiceOptions = new VisionServiceOptions("https://sikaimage.cognitiveservices.azure.com/", new AzureKeyCredential(configuration["SikaImage"]));
        }

        public async Task AnalyzeImage(byte[] bytes)
        {
            using var imageSourceBuffer = new ImageSourceBuffer();
            imageSourceBuffer.GetWriter().Write(new Memory<byte>(bytes));
            using var imageSource = VisionSource.FromImageSourceBuffer(imageSourceBuffer);

            var analysisOptions = new ImageAnalysisOptions()
            {
                Features =
                ImageAnalysisFeature.Objects
                | ImageAnalysisFeature.People
                | ImageAnalysisFeature.Text
                | ImageAnalysisFeature.Tags
            };

            using var analyzer = new ImageAnalyzer(visionServiceOptions, imageSource, analysisOptions);

            var result = await analyzer.AnalyzeAsync();

        }
        public async Task GetVectorOfImage(byte[] bytes)
        {
            var id = Guid.NewGuid();

            MemoryStream ms = new MemoryStream(bytes);

            string storageConnectionString = configuration["BlobConnection"];

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = account.CreateCloudBlobClient();

            // Make sure container is there
            var blobContainer = blobClient.GetContainerReference("default");
            await blobContainer.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            await blobContainer.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference($"{id.ToString()}.jpg");
            await blockBlob.UploadFromStreamAsync(ms);

            var blob = blobContainer.GetBlobReference($"{id.ToString()}.jpg");

            var uri = blob.Uri.AbsoluteUri;

            var client = new HttpClient();
            
            client.BaseAddress = new Uri("https://sikaimage.cognitiveservices.azure.com/");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration["SikaImage"]);

            using var content = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ImageUrl { Url = uri })))
            {
                Headers = { ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json) }
            };

            var v = await client.PostAsync("computervision/retrieval:vectorizeImage?api-version=2023-04-01-preview&modelVersion=latest", content);
           
        }
    }
    public class ImageUrl
    {
        public string Url { get; set; }

    }
}
